using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Models.ViewModels;
using Thss0.Web.Extensions;
using Thss0.Web.Models.Entities;
using Thss0.Web.Models;
using System.Data;

namespace Thss0.Web.Controllers.API
{
    [Route("api/professional")]
    [Route("api/client")]
    [ApiController]
    public class UserController(UserManager<ApplicationUser> um) : Controller
    {
        [HttpGet("{printBy:int?}/{page:int?}/{order:bool?}")]
        public async Task<ActionResult<Response>> Get(int printBy = 20, int page = 1, bool order = true)
        {
            var role = Request.Path.Value?.Split('/')[2];
            IList<ApplicationUser> users;
            if (role == "client")
            {
                users = await GetClients();
            }
            else
            {
                users = await GetProfessionals();
            }
            return Json(new Response
            {
                Content = (order ? users.OrderBy(u => u.Name) : users.OrderByDescending(u => u.Name))
                                            .Skip((page - 1) * printBy).Take(printBy)
                                            .Select(u => new ViewModel { Id = u.Id, Name = u.Name.Replace('_', ' ') })
                , TotalAmount = users.Count
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        private async Task<IList<ApplicationUser>> GetClients()
            => await um.GetUsersInRoleAsync("client");

        private async Task<IList<ApplicationUser>> GetProfessionals()
            => await um.GetUsersInRoleAsync("professional");

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<ActionResult<UserViewModel>> Get(string id)
        {
            var role = Request.Path.Value?.Split('/')[2] ?? "client";
            if (id == null || um.Users == null)
            {
                return NotFound();
            }
            var user = (await um.GetUsersInRoleAsync(role)).FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return await Initialize(user);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin, professional")]
        public async Task<ActionResult<UserViewModel>> Post(UserViewModel user)
        {
            new InitializationHelper().Validation(ModelState, user);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var toAdd = new ApplicationUser();
            Initialize(user, toAdd);
            var res = await um.CreateAsync(toAdd, user.Password);
            if (res.Succeeded)
            {
                await um.AddToRoleAsync(toAdd, user.Role);
                return Ok(user);
            }
            foreach (var err in res.Errors)
            {
                ModelState.AddModelError(err.Code, err.Description);
            }
            return BadRequest(ModelState);
        }

        private static readonly string[] _forbProps = ["Id", "Password"];
        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult<UserViewModel>> Put(string id, UserViewModel user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(user);
            }
            try
            {
                var toUpdate = await um.FindByIdAsync(id);
                if (toUpdate != null)
                {
                    var props = typeof(UserViewModel).GetProperties().Where(p => !_forbProps.Contains(p.Name)).ToArray();
                    for (int i = 0; i < props.Length; i++)
                    {
                        props[i].SetValue(toUpdate, props[i].GetValue(user));
                    }
                    toUpdate.PasswordHash = um.PasswordHasher.HashPassword(toUpdate, user.Password);
                    if (user.Role != "" && !await um.IsInRoleAsync(toUpdate, user.Role))
                    {
                        await um.RemoveFromRoleAsync(toUpdate, user.Role);
                        await um.AddToRoleAsync(toUpdate, user.Role);
                    }
                    var res = await um.UpdateAsync(toUpdate);
                    if (!res.Succeeded)
                    {
                        foreach (var err in res.Errors)
                        {
                            ModelState.AddModelError(err.Code, err.Description);
                        }
                        return BadRequest(ModelState);
                    }
                }
            }
            catch (Exception e)
            {
                if (!Exists(user.Email))
                {
                    return NotFound();
                }
                Console.WriteLine(e.Message);
            }
            return Ok(user);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult<UserViewModel>> Delete(string id)
        {
            if (um.Users == null)
            {
                return BadRequest(new { err = "Entity set \"dbo.AspNetUsers\" is null." });
            }
            var userToDelete = await um.FindByIdAsync(id);
            if (userToDelete != null)
            {
                var result = await um.DeleteAsync(userToDelete);
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

        private static void Initialize(UserViewModel src, ApplicationUser dest)
        {
            new InitializationHelper().InitializeEntity(src, dest);
        }

        private async Task<UserViewModel> Initialize(ApplicationUser src)
        {
            var dest = (UserViewModel)new InitializationHelper().InitializeViewModel(src, new UserViewModel());
            dest.Role = (await um.GetRolesAsync(src)).FirstOrDefault() ?? "No role";
            dest.Name = dest.Name.Replace('_', ' ');
            if (src.Procedure != null)
            {
                HandleProcedures(src, dest);
            }
            if (src.Result != null)
            {
                HandleResults(src, dest);
            }
            return dest;
        }
        private static void HandleProcedures(ApplicationUser src, UserViewModel dest)
        {
            for (int i = 0; i < src.Procedure.Count; i++)
            {
                dest.Procedure += $"{src.Procedure.ElementAtOrDefault(i)?.Id}\n";
                dest.ProcedureNames += $"{src.Procedure.ElementAtOrDefault(i)?.Name}\n";
            }
        }
        private static void HandleResults(ApplicationUser src, UserViewModel dest)
        {
            for (int i = 0; i < src.Procedure.Count; i++)
            {
                dest.Result += $"{src.Result.ElementAtOrDefault(i)?.Id}\n";
                dest.ResultNames += $"{src.Result.ElementAtOrDefault(i)?.ObtainmentTime}\n";
            }
        }

        private bool Exists(string email)
            => um.Users.Any(u => u.Email == email);
    }
}