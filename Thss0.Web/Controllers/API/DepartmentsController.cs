using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Data;
using Thss0.Web.Extensions;
using Thss0.Web.Models.Entities;
using Thss0.Web.Models.ViewModels;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string DEFAULT_ROLE = "client";

        public DepartmentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{order:bool?}/{printBy:int?}/{page:int?}")]
        public async Task<ActionResult<IEnumerable<DepartmentViewModel>>> GetDepartments(bool order = true, int printBy = 20, int page = 1)
        {
            var departments = await _context.Departments.ToListAsync();
            if (!departments.Any())
            {
                return NoContent();
            }
            return Json(new
            {
                content = (order ? departments.OrderBy(d => d.Name) : departments.OrderByDescending(d => d.Name))
                                                    .Skip((page - 1) * printBy).Take(printBy)
                                                    .Select(d => new DepartmentViewModel { Id = d.Id, Name = d.Name })
                , total_amount = await _context.Departments.CountAsync()
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentViewModel>> GetDepartment(string id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return InitializeDepartment(department);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult<Department>> PostDepartment(DepartmentViewModel department)
        {
            new EntityInitializer().Validation(ModelState, department);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var departmentToAdd = new Department();
            await InitializeDepartment(department, departmentToAdd);
            await _context.Departments.AddAsync(departmentToAdd);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                if (DepartmentExists(departmentToAdd.Id))
                {
                    return Conflict();
                }
            }
            return Ok(new { id = departmentToAdd.Id });
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult> PutDepartment(string id, DepartmentViewModel department)
        {
            if (id != department.Id)
            {
                return BadRequest();
            }
            var departmentToUpdate = await _context.Departments.FindAsync(id);
            if (departmentToUpdate == null)
            {
                return NotFound();
            }
            await InitializeDepartment(department, departmentToUpdate);
            _context.Entry(departmentToUpdate).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                Console.WriteLine(e.Message);
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult> DeleteDepartment(string id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            _context.Departments.Remove(department);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return NoContent();
        }

        private async Task<Department> InitializeDepartment(DepartmentViewModel source, Department dest)
        {
            new EntityInitializer().InitializeEntity(ModelState, source, dest);
            if (source.ProcedureNames != "")
            {
                await HandleProcedures(source, dest);
            }
            if (source.UserNames != "")
            {
                await HandleUsers(source, dest);
            }
            return dest;
        }
        private async Task<Department> HandleProcedures(DepartmentViewModel source, Department dest)
        {
            var proceduresArr = source.ProcedureNames.Split();
            for (ushort i = 0; i < proceduresArr.Length; i++)
            {
                dest.Procedure.Add(
                    await _context.Procedures.FirstOrDefaultAsync(p => p.Name == proceduresArr[i])
                    ?? new Procedure
                    {
                        Name = proceduresArr[i]
                    }
                );
            }
            return dest;
        }
        private async Task<Department> HandleUsers(DepartmentViewModel source, Department dest)
        {
            var usersArr = source.UserNames.Split();
            ApplicationUser user;
            IdentityResult result;
            for (ushort i = 0; i < usersArr.Length; i++)
            {
                user = await _userManager.FindByNameAsync(usersArr[i]);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = usersArr[i]
                    };
                    result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        foreach (var err in result.Errors)
                        {
                            ModelState.AddModelError(err.Code, err.Description);
                        }
                        return dest;
                    }
                    await _userManager.AddToRoleAsync(user, DEFAULT_ROLE);
                }
                dest.User.Add(await _userManager.FindByIdAsync(user.Id));
            }
            return dest;
        }

        private DepartmentViewModel InitializeDepartment(Department source)
        {
            var dest = (DepartmentViewModel)new EntityInitializer().InitializeViewModel(source, new DepartmentViewModel());
            if (source.Procedure != null)
            {
                HandleProcedures(source, dest);
            }
            if (source.User != null)
            {
                HandleUsers(source, dest);
            }
            return dest;
        }
        private void HandleProcedures(Department source, DepartmentViewModel dest)
        {
            var procedures = source.Procedure;
            for (ushort i = 0; i < procedures.Count; i++)
            {
                dest.Procedure += $"{procedures.ElementAtOrDefault(i)?.Id}\n";
                dest.ProcedureNames += $"{procedures.ElementAtOrDefault(i)?.Name}\n";
            }
        }
        private void HandleUsers(Department source, DepartmentViewModel dest)
        {
            var users = source.User;
            for (ushort i = 0; i < users.Count; i++)
            {
                dest.User += $"{users.ElementAtOrDefault(i)?.Id}\n";
                dest.UserNames += $"{users.ElementAtOrDefault(i)?.UserName}\n";
            }
        }
        private bool DepartmentExists(string id)
            => _context.Departments.Any(e => e.Id == id);
    }
}
