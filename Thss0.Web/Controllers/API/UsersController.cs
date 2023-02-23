using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Models.ViewModels;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _usrMngr;
        public UsersController(UserManager<IdentityUser> usrMngr)
        {
            _usrMngr = usrMngr;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewModel>> GetUser(string id)
        {
            if (id == null || _usrMngr.Users == null)
            {
                return NotFound();
            }
            var usrToGt = await _usrMngr.FindByIdAsync(id);
            if (usrToGt == null)
            {
                return NotFound();
            }
            var usrToRtrn = new UserViewModel
            {
                Id = usrToGt.Id,
                Name = usrToGt.UserName,
                PhoneNumber = usrToGt.PhoneNumber,
                Email = usrToGt.Email,
                Role = _usrMngr.GetRolesAsync(usrToGt).Result.FirstOrDefault() ?? "No role"
            };
            return usrToRtrn;
        }

        [HttpGet("{roleIndex:int?}")]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetUsers(int roleIndex = 0)
        {
            string role = "";
            switch (roleIndex)
            {
                case 0:
                    role = "client";
                    break;
                case 1:
                    role = "professional";
                    break;
                case 2:
                    role = "admin";
                    break;
            }
            return (await _usrMngr.GetUsersInRoleAsync(role)).Select(usr => new UserViewModel
                {
                    Id = usr.Id,
                    Name = usr.UserName,
                    PhoneNumber = usr.PhoneNumber,
                    Email = usr.Email,
                    Role = _usrMngr.GetRolesAsync(usr).Result.FirstOrDefault() ?? "No role"
                }).ToList();
        }

        [HttpPost]
        public async Task<ActionResult<UserViewModel>> Post(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                var usrToAdd = new IdentityUser
                {
                    UserName = user.Name,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email
                };
                var crteRslt = await _usrMngr.CreateAsync(usrToAdd, user.Password);
                if (!crteRslt.Succeeded)
                {
                    // for (int i = 0; i < crteRslt.Errors.Count; i++)
                    // {
                        foreach (var err in crteRslt.Errors)
                        {
                            ModelState.AddModelError(err.Code, err.Description);
                        }
                    // }
                    return BadRequest(ModelState);
                }
                await _usrMngr.AddToRoleAsync(usrToAdd, user.Role);
                return Ok(user);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
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
                    var usrToUpdte = await _usrMngr.FindByIdAsync(id);
                    if (usrToUpdte != null)
                    {
                        usrToUpdte.UserName = user.Name;
                        usrToUpdte.PasswordHash = _usrMngr.PasswordHasher.HashPassword(usrToUpdte, user.Password);
                        usrToUpdte.PhoneNumber = user.PhoneNumber;
                        usrToUpdte.Email = user.Email;
                        var idnttyRslt = await _usrMngr.UpdateAsync(usrToUpdte);
                        if (!idnttyRslt.Succeeded)
                        {
                            foreach (var err in idnttyRslt.Errors)
                            {
                                ModelState.AddModelError(err.Code, err.Description);
                            }
                            return BadRequest(ModelState);
                        }
                    }
                }
                catch (Exception)
                {
                    if (!UserExists(user.Email))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return Ok(user);
            }
            return BadRequest(user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<UserViewModel>> Delete(string id)
        {
            if (_usrMngr.Users == null)
            {
                return BadRequest(new { err = "Entity set 'dbo.AspNetUsers' is null." });
            }
            var usrToDlte = await _usrMngr.FindByIdAsync(id);
            if (usrToDlte != null)
            {
                var dlteRslt = await _usrMngr.DeleteAsync(usrToDlte);
                if (!dlteRslt.Succeeded)
                {
                    foreach (var err in dlteRslt.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok();
            }
            return NotFound();
        }

        private bool UserExists(string email)
            => _usrMngr.Users.Any(usr => usr.Email == email);
    }
}