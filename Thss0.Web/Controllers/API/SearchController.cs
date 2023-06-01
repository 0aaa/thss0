using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Thss0.Web.Data;
using Thss0.Web.Models;
using Thss0.Web.Models.ViewModels.CRUD;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            switch (entityName)
            {
                case "departments":
                    var departments = _context.Departments.Where(d => d.Name != null && d.Name.Contains(toFind));
                    var adjustedDepartments = await (order ? departments.OrderBy(d => d.Name) : departments.OrderByDescending(d => d.Name))
                                                            .Skip((page - 1) * printBy).Take(printBy).ToListAsync();
                    json = new
                    {
                        content = adjustedDepartments,
                        total_amount = adjustedDepartments.Count
                    };
                    break;
                case "clients":
                case "professionals":
                case "users"://
                    var properties = new[] { "Id", "UserName", "PhoneNumber", "Email", "DoB", "PoB"};
                    var destProperties = typeof(UserViewModel).GetProperties().Where(p => properties.Contains(p.Name)).ToArray();
                    var users = await _userManager.Users.Where(u => FindByString(u, destProperties, toFind)).ToListAsync();
                    var adjustedUsers = (order ? users.OrderBy(u => u.UserName) : users.OrderByDescending(u => u.UserName))
                                                            .Skip((page - 1) * printBy).Take(printBy)
                                                            .Select(u => InitializeUser(u, properties, destProperties).Result);
                    json = new
                    {
                        content = adjustedUsers,
                        total_amount = adjustedUsers.Count()
                    };
                    break;
                case "procedures":
                    var procedures = _context.Procedures.Where(p => p.Name != null && p.Name.Contains(toFind));
                    var adjustedProcedures = await (order ? procedures.OrderBy(p => p.Name) : procedures.OrderByDescending(p => p.Name))
                                                            .Skip((page - 1) * printBy).Take(printBy).ToListAsync();
                    json = new
                    {
                        content = adjustedProcedures,
                        total_amount = adjustedProcedures.Count
                    };
                    break;
                case "results":
                    var results = _context.Results.Where(r => r.Content != null && r.Content.Contains(toFind));
                    var adjustedResults = await (order ? results.OrderBy(r => r.ObtainmentTime) : results.OrderByDescending(r => r.ObtainmentTime))
                                                        .Skip((page - 1) * printBy).Take(printBy).ToListAsync();
                    json = new
                    {
                        content = adjustedResults,
                        total_amount = adjustedResults.Count
                    };
                    break;
                case "substances":
                    json = new
                    {
                        content = await new SubstancesController(_context).GetSubstance(toFind)
                    };
                    break;
                default:
                    return NoContent();
            }
            return Json(json);
        }

        private bool FindByString(ApplicationUser source, PropertyInfo[] destProperties, string toFind)
        {
            object? value;
            for (ushort i = 0; i < destProperties.Length; i++)
            {
                value = destProperties[i].GetValue(source);
                if (value != null && ((string)value).Contains(toFind))
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<UserViewModel> InitializeUser(ApplicationUser source, string[] properties, PropertyInfo[] destProperties)
        {
            var sourceProperties = typeof(ApplicationUser).GetProperties().Where(p => properties.Contains(p.Name)).ToArray();
            var dest = new UserViewModel();
            for (ushort i = 0; i < destProperties.Length; i++)
            {
                destProperties.First(p => p.Name == properties[i])
                        .SetValue(dest, destProperties.First(p => p.Name == properties[i]).GetValue(source));
            }
            dest.Role = (await _userManager.GetRolesAsync(source)).FirstOrDefault() ?? "No role";
            return dest;
        }
    }
}
