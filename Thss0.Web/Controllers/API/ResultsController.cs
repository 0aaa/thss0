using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using Thss0.Web.Data;
using Thss0.Web.Extensions;
using Thss0.Web.Models.Entities;
using Thss0.Web.Models.ViewModels.CRUD;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
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

        [HttpGet("{order:bool?}/{printBy:int?}/{page:int?}")]
        public async Task<ActionResult<IEnumerable<ResultViewModel>>> GetResults(bool order = true, int printBy = 20, int page = 1)
        {
            var results = await _context.Results.ToListAsync();
            if (!results.Any())
            {
                return NoContent();
            }
            return Json(new
            {
                content = (order ? results.OrderBy(r => r.ObtainmentTime) : results.OrderByDescending(r => r.ObtainmentTime))
                                                .Skip((page - 1) * printBy).Take(printBy)
                                                .Select(r => new ResultViewModel { Id = r.Id, ObtainmentTime = r.ObtainmentTime.ToString() })
                , total_amount = await _context.Results.CountAsync()
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultViewModel>> GetResult(string id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return InitializeResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<Result>> PostResult(ResultViewModel result)
        {
            new EntityInitializer().Validation(ModelState, result);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var resultToAdd = new Result();
            await InitializeResult(result, resultToAdd);
            await _context.Results.AddAsync(resultToAdd);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                if (ResultExists(resultToAdd.Id))
                {
                    return Conflict();
                }
            }
            return Ok(new { id = resultToAdd.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutResult(string id, ResultViewModel result)
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
            await InitializeResult(result, resultToUpdate);
            _context.Entry(resultToUpdate).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                Console.WriteLine(e.Message);
                if (!ResultExists(id))
                {
                    return NotFound();
                }
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResult(string id)
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

        private async Task<Result> InitializeResult(ResultViewModel source, Result dest)
        {
            new EntityInitializer().InitializeEntity(ModelState, source, dest);
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

        private ResultViewModel InitializeResult(Result source)
        {
            var dest = (ResultViewModel)new EntityInitializer().InitializeViewModel(source, new ResultViewModel());
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
                dest.UserNames += $"{users.ElementAtOrDefault(i)?.UserName}\n";
            }
        }

        private bool ResultExists(string id)
            => _context.Results.Any(e => e.Id == id);
    }
}
