using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Data;
using Thss0.Web.Extensions;
using Thss0.Web.Models;
using Thss0.Web.Models.Entities;
using Thss0.Web.Models.ViewModels;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController(ApplicationDbContext c, UserManager<ApplicationUser> um) : Controller
    {
        private const string DEFAULT_ROLE = "professional";

        [HttpGet("{printBy:int?}/{page:int?}/{order:bool?}")]
        public async Task<ActionResult<Response>> Get(int printBy = 20, int page = 1, bool order = true)
        {
            var departments = await c.Departments.ToListAsync();
            return Json(new Response
            {
                Content = (order ? departments.OrderBy(d => d.Name) : departments.OrderByDescending(d => d.Name))
                                                    .Skip((page - 1) * printBy).Take(printBy)
                                                    .Select(d => new ViewModel { Id = d.Id, Name = d.Name })
                , TotalAmount = departments.Count
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentViewModel>> Get(string id)
        {
            var d = await c.Departments.FindAsync(id);
            if (d == null)
            {
                return NotFound();
            }
            return Initialize(d);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult<Department>> Post(DepartmentViewModel department)
        {
            new InitializationHelper().Validation(ModelState, department);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var toAdd = new Department();
            await Initialize(department, toAdd);
            await c.Departments.AddAsync(toAdd);
            try
            {
                await c.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                if (Exists(toAdd.Id))
                {
                    return Conflict();
                }
            }
            return Ok(new { id = toAdd.Id });
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult> Put(string id, DepartmentViewModel department)
        {
            if (id != department.Id)
            {
                return BadRequest();
            }
            var toUpdate = await c.Departments.FindAsync(id);
            if (toUpdate == null)
            {
                return NotFound();
            }
            await Initialize(department, toUpdate);
            c.Entry(toUpdate).State = EntityState.Modified;
            try
            {
                await c.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                Console.WriteLine(e.Message);
                if (!Exists(id))
                {
                    return NotFound();
                }
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult> Delete(string id)
        {
            var d = await c.Departments.FindAsync(id);
            if (d == null)
            {
                return NotFound();
            }
            c.Departments.Remove(d);
            try
            {
                await c.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return NoContent();
        }

        private async Task<Department> Initialize(DepartmentViewModel src, Department dest)
        {
            new InitializationHelper().InitializeEntity(src, dest);
            if (src.ProcedureNames != "")
            {
                await HandleProcedures(src, dest);
            }
            if (src.ProfessionalNames != "")
            {
                await HandleUsers(src, dest);
            }
            return dest;
        }
        private async Task<Department> HandleProcedures(DepartmentViewModel src, Department dest)
        {
            var procedures = src.ProcedureNames.Split();
            for (int i = 0; i < procedures.Length; i++)
            {
                dest.Procedure.Add(
                    await c.Procedures.FirstOrDefaultAsync(p => p.Name == procedures[i])
                    ?? new Procedure
                    {
                        Name = procedures[i]
                    }
                );
            }
            return dest;
        }
        private async Task<Department> HandleUsers(DepartmentViewModel source, Department dest)
        {
            var users = source.ProfessionalNames.Split();
            ApplicationUser? u;
            IdentityResult ir;
            for (int i = 0; i < users.Length; i++)
            {
                u = await um.FindByNameAsync(users[i]);
                if (u == null)
                {
                    u = new ApplicationUser
                    {
                        Name = users[i]
                    };
                    ir = await um.CreateAsync(u);
                    if (!ir.Succeeded)
                    {
                        foreach (var err in ir.Errors)
                        {
                            ModelState.AddModelError(err.Code, err.Description);
                        }
                        return dest;
                    }
                    await um.AddToRoleAsync(u, DEFAULT_ROLE);
                }
                dest.User.Add(await um.FindByIdAsync(u.Id) ?? new());
            }
            return dest;
        }

        private static DepartmentViewModel Initialize(Department src)
        {
            var dest = (DepartmentViewModel)new InitializationHelper().InitializeViewModel(src, new DepartmentViewModel());
            if (src.Procedure != null)
            {
                HandleProcedures(src, dest);
            }
            if (src.User != null)
            {
                HandleUsers(src, dest);
            }
            return dest;
        }
        private static void HandleProcedures(Department src, DepartmentViewModel dest)
        {
            var procedures = src.Procedure;
            for (int i = 0; i < procedures.Count; i++)
            {
                dest.Procedure += $"{procedures.ElementAtOrDefault(i)?.Id}\n";
                dest.ProcedureNames += $"{procedures.ElementAtOrDefault(i)?.Name}\n";
            }
        }
        private static void HandleUsers(Department src, DepartmentViewModel dest)
        {
            var profs = src.User;
            for (int i = 0; i < profs.Count; i++)
            {
                dest.Professional += $"{profs.ElementAtOrDefault(i)?.Id}\n";
                dest.ProfessionalNames += $"{profs.ElementAtOrDefault(i)?.Name}\n";
            }
        }
        private bool Exists(string id)
            => c.Departments.Any(e => e.Id == id);
    }
}