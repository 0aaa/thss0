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
    public class ResultController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {
        [HttpGet("{printBy:int?}/{page:int?}/{order:bool?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<ActionResult<Response>> Get(int printBy = 20, int page = 1, bool order = true)
        {
            var results = await context.Results.ToListAsync();
            return Json(new Response
            {
                Content = (order ? results.OrderBy(r => r.Name) : results.OrderByDescending(r => r.Name))
                                                .Skip((page - 1) * printBy).Take(printBy)
                                                .Select(r => new ViewModel { Id = r.Id, Name = r.Name.ToString() })
                , TotalAmount = results.Count
            });
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<ActionResult<ResultViewModel>> Get(string id)
        {
            var result = await context.Results.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return await Initialize(result);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<ActionResult<Result>> Post(ResultViewModel result)
        {
            new InitializationHelper().Validation(ModelState, result);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultToAdd = new Result();
            await Initialize(result, resultToAdd);
            await context.Results.AddAsync(resultToAdd);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                if (Exists(resultToAdd.Id))
                {
                    return Conflict();
                }
            }
            return Ok(new { id = resultToAdd.Id });
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult> Put(string id, ResultViewModel result)
        {
            if (id != result.Id)
            {
                return BadRequest();
            }
            var resultToUpdate = await context.Results.FindAsync(id);
            if (resultToUpdate == null)
            {
                return NotFound();
            }
            await Initialize(result, resultToUpdate);
            context.Entry(resultToUpdate).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
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
        public async Task<ActionResult> DeleteResult(string id)
        {
            var result = await context.Results.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            context.Results.Remove(result);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return NoContent();
        }

        private async Task<Result> Initialize(ResultViewModel source, Result dest)
        {
            new InitializationHelper().InitializeEntity(source, dest);
            if (source.ProfessionalNames != "" || source.ClientNames != "")
            {
                await HandleUsers(source, dest);
            }
            if (source.ProcedureNames != "")
            {
                await HandleProcedures(source, dest);
            }
            return dest;
        }
        private async Task<Result> HandleUsers(ResultViewModel source, Result dest)
        {
            string[] usersArr = [.. source.ClientNames.Split(), .. source.ProfessionalNames.Split()];
            ApplicationUser? user;
            IdentityResult result;
            for (int i = 0; i < usersArr.Length; i++)
            {
                user = await userManager.FindByNameAsync(usersArr[i]);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Name = usersArr[i]
                    };
                    result = await userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        foreach (var err in result.Errors)
                        {
                            ModelState.AddModelError(err.Code, err.Description);
                        }
                        return dest;
                    }
                    await userManager.AddToRoleAsync(user, source.ProfessionalNames.Contains(usersArr[i]) ? "professional" : "client");
                }
                dest.User.Add(await userManager.FindByIdAsync(user.Id) ?? new());
            }
            return dest;
        }
        private async Task<Result> HandleProcedures(ResultViewModel source, Result dest)
        {
            dest.Procedure
                = await context.Procedures.FirstOrDefaultAsync(p => p.Name == source.ProcedureNames)
                ?? new Procedure
                {
                    Name = source.ProcedureNames
                };
            return dest;
        }

        private async Task<ResultViewModel> Initialize(Result source)
        {
            var dest = (ResultViewModel)new InitializationHelper().InitializeViewModel(source, new ResultViewModel());
            if (source.User != default)
            {
                await HandleUsers(source, dest);
            }
            if (source.Procedure != default)
            {
                dest.Procedure = source.Procedure.Id;
                dest.ProcedureNames = source.Procedure.Name;
            }
            return dest;
        }
        private async Task HandleUsers(Result source, ResultViewModel dest)
        {
            ApplicationUser currUser;
            for (int i = 0; i < source.User.Count; i++)
            {
                currUser = source.User.ElementAtOrDefault(i) ?? new();
                switch ((await userManager.GetRolesAsync(currUser))[0])
                {
                    case "client":
                        dest.Client += $"{currUser.Id}\n";
                        dest.ClientNames += $"{currUser.Name}\n";
                        break;
                    case "professional":
                        dest.Professional += $"{currUser.Id}\n";
                        dest.ProfessionalNames += $"{currUser.Name}\n";
                        break;
                }
            }
        }

        private bool Exists(string id)
            => context.Results.Any(e => e.Id == id);
    }
}
