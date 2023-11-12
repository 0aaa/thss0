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
    public class ResultsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string DEFAULT_ROLE = "client";

        public ResultsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{printBy:int?}/{page:int?}/{order:bool?}/{toFind?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<ActionResult<Response>> Get(int printBy = 20, int page = 1, bool order = true, string toFind = "")
        {
            IEnumerable<Result> results;
            if (toFind == "")
            {
                results = await _context.Results.ToListAsync();
            }
            else
            {
                results = await _context.Results.Where(r => r.Content != null && r.Content.Contains(toFind)).ToListAsync();
            }
            return Json(new Response
            {
                Content = (order ? results.OrderBy(r => r.ObtainmentTime) : results.OrderByDescending(r => r.ObtainmentTime))
                                                .Skip((page - 1) * printBy).Take(printBy)
                                                .Select(r => new ViewModel { Id = r.Id, Name = r.ObtainmentTime.ToString() })
                , TotalAmount = results.Count()
            });
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<ActionResult<ResultViewModel>> Get(string id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Initialize(result);
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
            await _context.Results.AddAsync(resultToAdd);
            try
            {
                await _context.SaveChangesAsync();
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
            var resultToUpdate = await _context.Results.FindAsync(id);
            if (resultToUpdate == null)
            {
                return NotFound();
            }
            await Initialize(result, resultToUpdate);
            _context.Entry(resultToUpdate).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
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
            var result = await _context.Results.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            _context.Results.Remove(result);
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

        private async Task<Result> Initialize(ResultViewModel source, Result dest)
        {
            new InitializationHelper().InitializeEntity(source, dest);
            if (source.UserNames != "")
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
                        Name = usersArr[i]
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
        private async Task<Result> HandleProcedures(ResultViewModel source, Result dest)
        {
            dest.Procedure
                = await _context.Procedures.FirstOrDefaultAsync(p => p.Name == source.ProcedureNames)
                ?? new Procedure
                {
                    Name = source.ProcedureNames
                };
            return dest;
        }

        private ResultViewModel Initialize(Result source)
        {
            var dest = (ResultViewModel)new InitializationHelper().InitializeViewModel(source, new ResultViewModel());
            if (source.User != default)
            {
                HandleUsers(source, dest);
            }
            if (source.Procedure != default)
            {
                dest.Procedure = source.Procedure.Id;
                dest.ProcedureNames = source.Procedure.Name;
            }
            return dest;
        }
        private void HandleUsers(Result source, ResultViewModel dest)
        {
            var users = source.User;
            for (ushort i = 0; i < users.Count; i++)
            {
                dest.User += $"{users.ElementAtOrDefault(i)?.Id}\n";
                dest.UserNames += $"{users.ElementAtOrDefault(i)?.Name}\n";
            }
        }

        private bool Exists(string id)
            => _context.Results.Any(e => e.Id == id);
    }
}
