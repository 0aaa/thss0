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
    public class ProcedureController(ApplicationDbContext c, UserManager<ApplicationUser> um) : Controller
    {
        [HttpGet("{printBy:int?}/{page:int?}/{order:bool?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<ActionResult<Response>> Get(int printBy = 20, int page = 1, bool order = true)
        {
            var procedures = await c.Procedures.ToListAsync();
            return Json(new Response
            {
                Content = (order ? procedures.OrderBy(p => p.Name) : procedures.OrderByDescending(p => p.Name))
                                                .Skip((page - 1) * printBy).Take(printBy)
                                                .Select(p => new ViewModel { Id = p.Id, Name = p.Name })
                , TotalAmount = procedures.Count
            });
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<ActionResult<ProcedureViewModel>> Get(string id)
        {
            var src = await c.Procedures.FindAsync(id);
            if (src == null)
            {
                return NotFound();
            }
            return await Initialize(src);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<ActionResult<Procedure>> Post(ProcedureViewModel procedure)
        {
            new InitializationHelper().Validation(ModelState, procedure);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var toAdd = new Procedure();
            await Initialize(procedure, toAdd);
            await c.Procedures.AddAsync(toAdd);
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
        public async Task<ActionResult> Put(string id, ProcedureViewModel procedure)
        {
            if (id != procedure.Id)
            {
                return BadRequest();
            }
            var toUpdate = await c.Procedures.FindAsync(id);
            if (toUpdate == null)
            {
                return NotFound();
            }
            await Initialize(procedure, toUpdate);
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
            var p = await c.Procedures.FindAsync(id);
            if (p == null)
            {
                return NotFound();
            }
            c.Procedures.Remove(p);
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

        private async Task<Procedure> Initialize(ProcedureViewModel src, Procedure dest)
        {
            new InitializationHelper().InitializeEntity(src, dest);
            if (src.DepartmentNames != "")
            {
                await HandleDepartment(src, dest);
            }
            if (src.ProfessionalNames != "" || src.ClientNames != "")
            {
                await HandleUsers(src, dest);
            }
            if (src.ResultNames != "")// Must follow after HandleUsers().
            {
                await HandleResult(src, dest);
            }
            if (src.Substance != "")
            {
                HandleSubstances(src, dest);// If Not disposed Context Exception occurs change the return type to Task<Procedure>.
            }
            return dest;
        }
        private async Task<Procedure> HandleDepartment(ProcedureViewModel src, Procedure dest)
        {
            dest.Department
                = await c.Departments.FirstOrDefaultAsync(d => d.Name == src.DepartmentNames)
                ?? new Department
                {
                    Name = src.DepartmentNames
                };
            return dest;
        }
        private async Task<Procedure> HandleResult(ProcedureViewModel source, Procedure dest)
        {
            var result = await c.Results.FirstOrDefaultAsync(r => r.ObtainmentTime.ToString() == source.ResultNames);
            string[] users = [.. source.ClientNames.Split(), .. source.ProfessionalNames.Split()];
            try
            {
                result ??= new Result
                {
                    ObtainmentTime = DateTime.Parse(source.ResultNames)
                };
                for (int i = 0; i < users.Length; i++)
                {
                    result.User.Add(await um.FindByNameAsync(users[i]) ?? new());
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            dest.Result = result;
            return dest;
        }
        private async Task<Procedure> HandleUsers(ProcedureViewModel src, Procedure dest)
        {
            string[] users = [.. src.ClientNames.Split(), .. src.ProfessionalNames.Split()];
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
                    await um.AddToRoleAsync(u, src.ProfessionalNames.Contains(users[i]) ? "professional" : "client");
                }
                dest.User.Add(await um.FindByIdAsync(u.Id) ?? new());
            }
            return dest;
        }
        private async void HandleSubstances(ProcedureViewModel src, Procedure dest)
        {
            var sc = new SubstanceController(c);
            var brands = src.Substance.Split().Distinct();
            ActionResult<SubstanceViewModel> res;
            for (int i = 0; i < brands.Count(); i++)
            {
                // res = await substancesCtrl.GetSubstance(brandNames.ElementAt(i), false);
                res = await sc.Get(brands.ElementAt(i));
                if (res.Value != null)
                {
                    dest.Substance.Add(new() { Id = res.Value.Id });
                }
            }
        }

        private async Task<ProcedureViewModel> Initialize(Procedure src)
        {
            var dest = (ProcedureViewModel)new InitializationHelper().InitializeViewModel(src, new ProcedureViewModel());
            if (src.Department != default)
            {
                dest.Department = src.Department.Id;
                dest.DepartmentNames = src.Department.Name;
            }
            if (src.Result != default)
            {
                dest.Result = src.Result.Id;
                dest.ResultNames = src.Result.ObtainmentTime.ToString();
            }
            if (src.User.Count > 0)
            {
                await HandleUsers(src, dest);
            }
            if (src.Substance.Count > 0)
            {
                await HandleSubstances(src, dest);
            }
            return dest;
        }
        private async Task HandleUsers(Procedure src, ProcedureViewModel dest)
        {
            ApplicationUser u;
            for (int i = 0; i < src.User.Count; i++)
            {
                u = src.User.ElementAtOrDefault(i) ?? new();
                switch ((await um.GetRolesAsync(u))[0])
                {
                    case "client":
                        dest.Client += $"{u.Id}\n";
                        dest.ClientNames += $"{u.Name}\n";
                        break;
                    case "professional":
                        dest.Professional += $"{u.Id}\n";
                        dest.ProfessionalNames += $"{u.Name}\n";
                        break;
                }
            }
        }
        private async Task HandleSubstances(Procedure src, ProcedureViewModel dest)
        {
            //var res = (await new SubstancesController(_context).GetSubstances(source.Id)).Value;
            //if (res != null)
            //{
            //    dest.Substance = res;
            //}
            var substances = src.Substance;
            var sc = new SubstanceController(c);
            SubstanceViewModel substance;
            for (int i = 0; i < substances.Count; i++)
            {
                substance = (await sc.Get(substances.ElementAtOrDefault(i)?.Id ?? "")).Value!;
                dest.Substance += $"{substance.Id}\n";
                dest.SubstanceNames += $"{substance.Name}\n";
            }
        }

        private bool Exists(string id)
            => c.Procedures.Any(e => e.Id == id);
    }
}