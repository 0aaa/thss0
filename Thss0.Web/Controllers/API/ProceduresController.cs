﻿using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Data;
using Thss0.Web.Extensions;
using Thss0.Web.Models;
using Thss0.Web.Models.ViewModels.CRUD;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
    public class ProceduresController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string DEFAULT_ROLE = "client";

        public ProceduresController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("{order:bool?}/{printBy:int?}/{page:int?}")]
        public async Task<ActionResult<IEnumerable<ProcedureViewModel>>> GetProcedures(bool order = true, int printBy = 20, int page = 1)
        {
            var procedures = await _context.Procedures.ToListAsync();
            if (!procedures.Any())
            {
                return NoContent();
            }
            return Json(new
            {
                content = (order ? procedures.OrderBy(p => p.Name) : procedures.OrderByDescending(p => p.Name))
                                            .Skip((page - 1) * printBy).Take(printBy)
                                            .Select(p => new ProcedureViewModel { Id = p.Id, Name = p.Name })
                , total_amount = await _context.Procedures.CountAsync()
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProcedureViewModel>> GetProcedure(string id)
        {
            var source = await _context.Procedures.FindAsync(id);
            if (source == null)
            {
                return NotFound();
            }
            return InitializeProcedure(source);
        }

        [HttpPost]
        public async Task<ActionResult<Procedure>> PostProcedure(ProcedureViewModel procedure)
        {
            // Validation(procedure);
            new EntityInitializer().Validation(ModelState, procedure);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var procedureToAdd = new Procedure();
            await InitializeProcedure(procedure, procedureToAdd);
            await _context.Procedures.AddAsync(procedureToAdd);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Message);
                if (ProcedureExists(procedureToAdd.Id))
                {
                    return Conflict();
                }
            }
            return Ok(new { id = procedureToAdd.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProcedure(string id, ProcedureViewModel procedure)
        {
            if (id != procedure.Id)
            {
                return BadRequest();
            }
            var procedureToUpdate = await _context.Procedures.FindAsync(id);
            if (procedureToUpdate == null)
            {
                return NotFound();
            }
            await InitializeProcedure(procedure, procedureToUpdate);
            _context.Entry(procedureToUpdate).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                Console.WriteLine(e.Message);
                if (!ProcedureExists(id))
                {
                    return NotFound();
                }
            }
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcedure(string id)
        {
            var procedure = await _context.Procedures.FindAsync(id);
            if (procedure == null)
            {
                return NotFound();
            }
            _context.Procedures.Remove(procedure);
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

        private void Validation(ProcedureViewModel procedure)
        {
            var properties = typeof(ProcedureViewModel).GetProperties()
                                    .Where(p => !new[] { "Id", "Department", "CreationTime", "Result", "ResultNames", "User", "Substance" }
                                    .Contains(p.Name)).ToArray();
            string value;
            for (ushort i = 0; i < properties.Length; i++)
            {
                value = properties[i].GetValue(procedure)?.ToString() ?? "";
                if (properties[i].Name.Contains("Time") && value != "" && DateTime.Parse(value) < DateTime.Now)
                {
                    ModelState.AddModelError(properties[i].Name, $"{Regex.Replace(properties[i].Name, "([a-z])([A-Z])", "$1 $2")} cannot be less than the current time");
                }
                else if (!properties[i].Name.Contains("Time") && value == "")
                {
                    ModelState.AddModelError(properties[i].Name, $"{Regex.Replace(properties[i].Name, "([a-z])([A-Z])", "$1 $2")} required");
                }
            }
        }

        private async Task<Procedure> InitializeProcedure(ProcedureViewModel source, Procedure dest)
        {
            // var properties = new[] { "Name", "RealizationTime", "NextProcedureTime" };
            // var sourceProperties = typeof(ProcedureViewModel).GetProperties()
            //                                 .Where(p => properties.Contains(p.Name)).ToArray();
            // var destProperties = typeof(Procedure).GetProperties()
            //                                 .Where(p => properties.Contains(p.Name)).ToArray();

            // for (ushort i = 0; i < properties.Length; i++)
            // {
            //     if (sourceProperties[i].GetValue(source)?.ToString() != "")
            //     {
            //         if (destProperties[i].PropertyType.Name == "DateTime")
            //         {
            //             destProperties.FirstOrDefault(p => p.Name == properties[i])
            //                     ?.SetValue(dest, DateTime.Parse(sourceProperties.FirstOrDefault(p => p.Name == properties[i])
            //                                                             ?.GetValue(source)?.ToString() ?? "0"));
            //         }
            //         else
            //         {
            //             destProperties.FirstOrDefault(p => p.Name == properties[i])
            //                     ?.SetValue(dest, sourceProperties.FirstOrDefault(p => p.Name == properties[i])?.GetValue(source));
            //         }
            //     }
            // }
            new EntityInitializer().InitializeEntity(ModelState, source, dest);
            if (source.DepartmentNames != "")
            {
                await HandleDepartment(source, dest);
            }
            if (source.UserNames != "")
            {
                await HandleUsers(source, dest);
            }
            if (source.ResultNames != "")// Must follow after HandleUsers().
            {
                await HandleResult(source, dest);
            }
            if (source.Substance != "")
            {
                HandleSubstances(source, dest);// If Not disposed Context Exception occurs change the return type to Task<Procedure>.
            }
            return dest;
        }
        private async Task<Procedure> HandleDepartment(ProcedureViewModel source, Procedure dest)
        {
            dest.Department
                = await _context.Departments.FirstOrDefaultAsync(d => d.Name == source.DepartmentNames)
                ?? new Department
                {
                    Name = source.DepartmentNames
                };
            return dest;
        }
        private async Task<Procedure> HandleResult(ProcedureViewModel source, Procedure dest)// Check for DateTime.Parse an invalid String.
        {
            var result = await _context.Results.FirstOrDefaultAsync(r => r.ObtainmentTime.ToString() == source.ResultNames);
            var usersArr = source.UserNames.Split();
            result ??= new Result
            {
                ObtainmentTime = DateTime.Parse(source.ResultNames)
            };
            for (ushort i = 0; i < usersArr.Length; i++)
            {
                result.User.Add(await _userManager.FindByNameAsync(usersArr[i]));
            }
            dest.Result = result;
            return dest;
        }
        private async Task<Procedure> HandleUsers(ProcedureViewModel source, Procedure dest)
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
        private async void HandleSubstances(ProcedureViewModel source, Procedure dest)
        {
            var substancesCtrl = new SubstancesController(_context);
            var brandNames = source.Substance.Split().Distinct();
            ActionResult<string> res;
            for (ushort i = 0; i < brandNames.Count(); i++)
            {
                res = await substancesCtrl.GetSubstance(brandNames.ElementAt(i), false);
                if (res.Value != null)
                {
                    dest.Substance.Add(new Substance
                    {
                        Id = res.Value
                    });
                }
            }
        }

        private ProcedureViewModel InitializeProcedure(Procedure source)
        {
            // var dest = new ProcedureViewModel();
            // var properties = new[] { "Id", "Name", "CreationTime", "RealizationTime", "NextProcedureTime" };
            // var sourceProperties = typeof(Procedure).GetProperties()
            //                                 .Where(p => properties.Contains(p.Name)).ToArray();
            // var destProperties = typeof(ProcedureViewModel).GetProperties()
            //                                 .Where(p => properties.Contains(p.Name)).ToArray();
            // for (ushort i = 0; i < properties.Length; i++)
            // {
            //     var value = sourceProperties[i].GetValue(source)?.ToString();
            //     if (value != default && value != default(DateTime).ToString())
            //     {
            //         destProperties.FirstOrDefault(p => p.Name == properties[i])
            //                 ?.SetValue(dest, sourceProperties.FirstOrDefault(p => p.Name == properties[i])?.GetValue(source)?.ToString());
            //     }
            // }
            var dest = (ProcedureViewModel)new EntityInitializer().InitializeViewModel(source, new ProcedureViewModel());
            if (source.Department != default)
            {
                dest.Department = source.Department.Id;
                dest.DepartmentNames = source.Department.Name;
            }
            if (source.Result != default)
            {
                dest.Result = source.Result.Id;
                dest.ResultNames = source.Result.ObtainmentTime.ToString();
            }
            if (source.User.Any())
            {
                HandleUsers(source, dest);
            }
            if (source.Substance.Any())
            {
                HandleSubstances(source, dest);
            }
            return dest;
        }
        private void HandleUsers(Procedure source, ProcedureViewModel dest)
        {
            var users = source.User;
            for (ushort i = 0; i < users.Count; i++)
            {
                dest.User += $"{users.ElementAtOrDefault(i)?.Id}\n";
                dest.UserNames += $"{users.ElementAtOrDefault(i)?.UserName}\n";
            }
        }
        private async void HandleSubstances(Procedure source, ProcedureViewModel dest)
        {
            var res = (await new SubstancesController(_context).GetSubstances(source.Id)).Value;
            if (res != null)
            {
                dest.Substance = res;
            }
        }

        private bool ProcedureExists(string id)
            => _context.Procedures.Any(e => e.Id == id);
    }
}