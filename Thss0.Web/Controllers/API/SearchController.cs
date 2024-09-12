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
    public partial class SearchController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly List<ViewModel?> _totalResults = [];
        private readonly string[] _entityTypes = ["departments", "users", "procedures", "results", "substances"];
        private int _typesCnt = 0;
        private IEnumerable<ViewModel?>? _searchResult;

        [HttpGet("{toFind}/{searchEntity?}/{printBy:int?}/{page:int?}/{order:bool?}")]
        public async Task<ActionResult<Response>> GetResults(string toFind, string searchEntity = "", int printBy = 20, int page = 1, bool order = true)
        {
            var res = new Response();
            toFind = HttpUtility.UrlDecode(toFind);
            switch (searchEntity)
            {
                case "departments":
                case "departmentNames":
                    var departments = await context.Departments.Where(d => d.Name != null && d.Name.Contains(toFind)).ToListAsync();
                    res = AdjustResults(departments.Cast<IEntity>(), printBy, page, order);
                    break;
                case "clients":
                case "professionals":
                case "users":
                case "userNames":
                    var properties = new[] { "Id", "UserName", "PhoneNumber", "Email", "DoB", "PoB" };
                    var sourceProperties = typeof(ApplicationUser).GetProperties().Where(p => properties.Contains(p.Name)).ToArray();
                    List<ApplicationUser> users;
                    if (searchEntity == "professionals" || searchEntity == "clients")
                    {
                        users = [.. (await userManager.GetUsersInRoleAsync(RoleName().Replace(searchEntity, "")))];
                    }
                    else
                    {
                        users = await userManager.Users.ToListAsync();
                    }
                    users = users.Where(u => FindByString(u, sourceProperties, toFind)).ToList();
                    res = AdjustResults(users.Cast<IEntity>(), printBy, page, order);
                    searchEntity = "users";
                    break;
                case "procedures":
                case "procedureNames":
                    var procedures = await context.Procedures.Where(p => p.Name != null && p.Name.Contains(toFind)).ToListAsync();
                    res = AdjustResults(procedures.Cast<IEntity>(), printBy, page, order);
                    break;
                case "results":
                case "resultNames":
                    //var results = await _context.Results.Where(r => r.Content != null && r.Content.Contains(toFind)).ToListAsync();
                    //res = AdjustResults(results.Cast<IEntity>(), printBy, page, order);
                    break;
                case "substances":
                case "substanceNames":
                    var substance = await new SubstanceController(context).Get(toFind, FdaId().IsMatch(toFind));
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
                    res = substances;
                    break;
            }
            if ((_typesCnt > 0 && _typesCnt < _entityTypes.Length) || !_entityTypes.Contains(searchEntity))
            {
                _searchResult = ((Response?)((JsonResult?)(await GetResults(toFind, _entityTypes[_typesCnt++])).Result)?.Value)?.Content;
                for (int i = 0; i < _searchResult?.Count(); i++)
                {
                    _totalResults.Add(_searchResult?.ElementAt(i));
                }
                if (!_entityTypes.Contains(searchEntity))
                {
                    res = new Response { Content = _totalResults, TotalAmount = _totalResults.Count };
                }
            }
            return Json(res);
        }

        private static bool FindByString(ApplicationUser source, PropertyInfo[] sourceProperties, string toFind)
        {
            string value;
            for (int i = 0; i < sourceProperties.Length; i++)
            {
                value = sourceProperties[i].GetValue(source)?.ToString() ?? "";
                if (value != "" && value.Contains(toFind))
                {
                    return true;
                }
            }
            return false;
        }

        private static Response AdjustResults(IEnumerable<IEntity> source, int printBy = 20, int page = 1, bool order = true)
        {
            return new Response
            {
                Content = (order ? source.OrderBy(p => p.Name) : source.OrderByDescending(p => p.Name))
                                                .Skip((page - 1) * printBy).Take(printBy)
                                                .Select(p => new ViewModel { Id = p.Id, Name = p.Name })
                , TotalAmount = source.Count()
            };
        }

        [GeneratedRegex(".$")]
        private static partial Regex RoleName();
        [GeneratedRegex("^[0-9]{4,5}-[0-9]{3,4}_[a-fA-F0-9]{8}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{4}-[a-fA-F0-9]{12}$")]
        private static partial Regex FdaId();
    }
}
