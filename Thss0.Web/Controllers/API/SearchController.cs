using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Thss0.Web.Data;
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

        public SearchController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{entityName}/{toFind}/{order:bool?}/{printBy:int?}/{page:int?}")]
        public async Task<ActionResult<IEnumerable<object>>> GetResults(string entityName, string toFind, bool order = true, int printBy = 20, int page = 1)
        {
            object json;
            toFind = HttpUtility.UrlDecode(toFind);
            switch (entityName)
            {
                case "departments":
                case "departmentNames":
                    var departments = await _context.Departments.Where(d => d.Name != null && d.Name.Contains(toFind)).ToListAsync();
                    var adjustedDepartments = (order ? departments.OrderBy(d => d.Name) : departments.OrderByDescending(d => d.Name))
                                                    .Skip((page - 1) * printBy).Take(printBy)
                                                    .Select(d => new DepartmentViewModel { Id = d.Id, Name = d.Name });
                    json = new
                    {
                        content = adjustedDepartments
                        , total_amount = adjustedDepartments.Count()
                    };
                    break;
                case "clients":
                case "professionals":
                case "users":
                case "userNames":
                    var properties = new[] { "Id", "UserName", "PhoneNumber", "Email", "DoB", "PoB"};
                    var sourceProperties = typeof(ApplicationUser).GetProperties().Where(p => properties.Contains(p.Name)).ToArray();
                    var users = new List<ApplicationUser>();
                    if (entityName == "professionals" || entityName == "clients")
                    {
                        users = (await _userManager.GetUsersInRoleAsync(Regex.Replace(entityName, ".$", ""))).ToList();
                    }
                    else
                    {
                        users = await _userManager.Users.ToListAsync();
                    }
                    users = users.Where(u => FindByString(u, sourceProperties, toFind)).ToList();
                    var adjustedUsers = (order ? users.OrderBy(u => u.UserName) : users.OrderByDescending(u => u.UserName))
                                            .Skip((page - 1) * printBy).Take(printBy)
                                            .Select(u => new UserViewModel { Id = u.Id, UserName = u.UserName });
                    json = new
                    {
                        content = adjustedUsers
                        , total_amount = adjustedUsers.Count()
                    };
                    break;
                case "procedures":
                case "procedureNames":
                    var procedures = await _context.Procedures.Where(p => p.Name != null && p.Name.Contains(toFind)).ToListAsync();
                    var adjustedProcedures = (order ? procedures.OrderBy(p => p.Name) : procedures.OrderByDescending(p => p.Name))
                                            .Skip((page - 1) * printBy).Take(printBy)
                                            .Select(p => new ProcedureViewModel { Id = p.Id, Name = p.Name });
                    json = new
                    {
                        content = adjustedProcedures
                        , total_amount = adjustedProcedures.Count()
                    };
                    break;
                case "results":
                case "resultNames":
                    var results = await _context.Results.Where(r => r.Content != null && r.Content.Contains(toFind)).ToListAsync();
                    var adjustedResults = (order ? results.OrderBy(r => r.ObtainmentTime) : results.OrderByDescending(r => r.ObtainmentTime))
                                                .Skip((page - 1) * printBy).Take(printBy)
                                                .Select(r => new ResultViewModel { Id = r.Id, ObtainmentTime = r.ObtainmentTime.ToString(), Content = r.Content });
                    json = new
                    {
                        content = adjustedResults
                        , total_amount = adjustedResults.Count()
                    };
                    break;
                case "substances":
                case "substanceNames":
                    json = new
                    {
                        content = new List<SubstanceViewModel>() { (await new SubstancesController(_context).GetSubstance(toFind, false)).Value ?? new SubstanceViewModel() }
                    };
                    break;
                default:
                    return NoContent();
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
