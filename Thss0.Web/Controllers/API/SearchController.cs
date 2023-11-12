using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Thss0.Web.Data;
using Thss0.Web.Models;
using Thss0.Web.Models.Entities;
using Thss0.Web.Models.ViewModels;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly List<ViewModel?> _totalResults;
        private readonly string[] _entityTypes;
        private int _typesCnt;
        private IEnumerable<ViewModel?>? _searchResult;

        public SearchController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _entityTypes = new[] { "departments", "users", "procedures", "results", "substances" };
            _typesCnt = 0;
            _totalResults = new List<ViewModel?>();
        }

        [HttpGet("{entityName}/{toFind}/{printBy:int?}/{page:int?}/{order:bool?}")]
        public async Task<ActionResult<Response>> GetResults(string entityName, string toFind, int printBy = 20, int page = 1, bool order = true)
        {
            object? json = null;
            toFind = HttpUtility.UrlDecode(toFind);
            switch (entityName)
            {
                case "departments":
                case "departmentNames":
                    json = (Response?)((JsonResult?)(await new DepartmentsController(_context, _userManager).Get(toFind: toFind)).Result)?.Value;
                    break;
                case "clients":
                case "professionals":
                case "users":
                case "userNames":
                    var properties = new[] { "Id", "UserName", "PhoneNumber", "Email", "DoB", "PoB" };
                    var sourceProperties = typeof(ApplicationUser).GetProperties().Where(p => properties.Contains(p.Name)).ToArray();
                    List<ApplicationUser> users;
                    if (entityName == "professionals" || entityName == "clients")
                    {
                        users = (await _userManager.GetUsersInRoleAsync(Regex.Replace(entityName, ".$", ""))).ToList();
                    }
                    else
                    {
                        users = await _userManager.Users.ToListAsync();
                    }
                    users = users.Where(u => FindByString(u, sourceProperties, toFind)).ToList();
                    var adjustedUsers = (order ? users.OrderBy(u => u.Name) : users.OrderByDescending(u => u.Name))
                                            .Skip((page - 1) * printBy).Take(printBy)
                                            .Select(u => new ViewModel { Id = u.Id, Name = u.Name });
                    json = new Response
                    {
                        Content = adjustedUsers
                        , TotalAmount = adjustedUsers.Count()
                    };
                    break;
                case "procedures":
                case "procedureNames":
                    json = (Response?)((JsonResult?)(await new ProceduresController(_context, _userManager).Get(toFind: toFind)).Result)?.Value;
                    break;
                case "results":
                case "resultNames":
                    json = (Response?)((JsonResult?)(await new ResultsController(_context, _userManager).Get(toFind: toFind)).Result)?.Value;
                    break;
                case "substances":
                case "substanceNames":
                    var substance = await new SubstancesController(_context).Get(toFind, false);
                    var substances = new Response
                    {
                        Content = new List<SubstanceViewModel>().AsEnumerable()
                        , TotalAmount = 0
                    };
                    if (substance.Value != null)
                    {
                        ((List<SubstanceViewModel>)substances.Content).Add(substance.Value);
                        substances.TotalAmount = substances.Content.Count();
                    }
                    json = substances;
                    break;
            }
            if ((_typesCnt > 0 && _typesCnt < _entityTypes.Length) || !_entityTypes.Contains(entityName))
            {
                _searchResult = ((Response?)((JsonResult?)(await GetResults(_entityTypes[_typesCnt++], toFind)).Result)?.Value)?.Content;
                for (int i = 0; i < _searchResult?.Count(); i++)
                {
                    _totalResults.Add(_searchResult?.ElementAt(i));
                }
                if (!_entityTypes.Contains(entityName))
                {
                    json = new Response { Content = _totalResults, TotalAmount = _totalResults.Count };
                }
            }
            return Json(json);
        }

        private bool FindByString(ApplicationUser source, PropertyInfo[] sourceProperties, string toFind)
        {
            string value;
            for (ushort i = 0; i < sourceProperties.Length; i++)
            {
                value = sourceProperties[i].GetValue(source)?.ToString() ?? "";
                if (value != "" && value.Contains(toFind))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
