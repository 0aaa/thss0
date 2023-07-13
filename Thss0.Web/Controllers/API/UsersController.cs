using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Models.ViewModels;
using Thss0.Web.Extensions;
using Thss0.Web.Models.Entities;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
            => _userManager = userManager;

        [HttpGet("{role?}/{order:bool?}/{printBy:int?}/{page:int?}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetUsers(string role = "client", bool order = true, int printBy = 20, int page = 1)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            if (!users.Any())
            {
                return NoContent();
            }
            return Json(new
            {
                content = (order ? users.OrderBy(u => u.UserName) : users.OrderByDescending(u => u.UserName))
                                            .Skip((page - 1) * printBy).Take(printBy)
                                            .Select(u => new UserViewModel { Id = u.Id, UserName = u.UserName })
                , total_amount = users.Count
            });
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<ActionResult<UserViewModel>> GetUser(string id)
        {
            if (id == null || _userManager.Users == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return await InitializeUser(user);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<ActionResult<UserViewModel>> Post(UserViewModel user)
        {
            new EntityInitializer().Validation(ModelState, user);
            if (ModelState.IsValid)
            {
                var userToAdd = new ApplicationUser
                {
                    UserName = user.UserName
                    , PhoneNumber = user.PhoneNumber
                    , Email = user.Email
                };
                var result = await _userManager.CreateAsync(userToAdd, user.Password);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                    }
                    return BadRequest(ModelState);
                }
                await _userManager.AddToRoleAsync(userToAdd, user.Role);
                return Ok(user);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult<UserViewModel>> Put(string id, UserViewModel user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var userToUpdate = await _userManager.FindByIdAsync(id);
                    if (userToUpdate != null)
                    {
                        var properties = typeof(UserViewModel).GetProperties().Where(p => !new[] { "Id", "Password" }.Contains(p.Name)).ToArray();
                        for (ushort i = 0; i < properties.Length; i++)
                        {
                            properties[i].SetValue(userToUpdate, properties[i].GetValue(user));
                        }
                        userToUpdate.PasswordHash = _userManager.PasswordHasher.HashPassword(userToUpdate, user.Password);
                        if (user.Role != "" && !await _userManager.IsInRoleAsync(userToUpdate, user.Role))
                        {
                            await _userManager.RemoveFromRoleAsync(userToUpdate, user.Role);
                            await _userManager.AddToRoleAsync(userToUpdate, user.Role);
                        }
                        var result = await _userManager.UpdateAsync(userToUpdate);
                        if (!result.Succeeded)
                        {
                            foreach (var err in result.Errors)
                            {
                                ModelState.AddModelError(err.Code, err.Description);
                            }
                            return BadRequest(ModelState);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (!UserExists(user.Email))
                    {
                        return NotFound();
                    }
                    Console.WriteLine(e.Message);
                }
                return Ok(user);
            }
            return BadRequest(user);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult<UserViewModel>> Delete(string id)
        {
            if (_userManager.Users == null)
            {
                return BadRequest(new { err = "Entity set 'dbo.AspNetUsers' is null." });
            }
            var userToDelete = await _userManager.FindByIdAsync(id);
            if (userToDelete != null)
            {
                var result = await _userManager.DeleteAsync(userToDelete);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok();
            }
            return NotFound();
        }

        private async Task<UserViewModel> InitializeUser(ApplicationUser source)
        {
            var dest = (UserViewModel)new EntityInitializer().InitializeViewModel(source, new UserViewModel());
            dest.Role = (await _userManager.GetRolesAsync(source)).FirstOrDefault() ?? "No role";
            if (source.Procedure != null)
            {
                HandleProcedures(source, dest);
            }
            if (source.Result != null)
            {
                HandleResults(source, dest);
            }
            return dest;
        }
        private void HandleProcedures(ApplicationUser source, UserViewModel dest)
        {
            for (ushort i = 0; i < source.Procedure.Count; i++)
            {
                dest.Procedure += $"{source.Procedure.ElementAtOrDefault(i)?.Id}\n";
                dest.ProcedureNames += $"{source.Procedure.ElementAtOrDefault(i)?.Name}\n";
            }
        }
        private void HandleResults(ApplicationUser source, UserViewModel dest)
        {
            for (ushort i = 0; i < source.Procedure.Count; i++)
            {
                dest.Result += $"{source.Result.ElementAtOrDefault(i)?.Id}\n";
                dest.ResultNames += $"{source.Result.ElementAtOrDefault(i)?.ObtainmentTime}\n";
            }
        }

        private bool UserExists(string email)
            => _userManager.Users.Any(u => u.Email == email);
    }
}