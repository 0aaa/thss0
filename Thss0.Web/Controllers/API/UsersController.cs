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

        [HttpGet("{role}")]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetUsers(string role)
            => _usrMngr.GetUsersInRoleAsync(role).Result.Select(usr => new UserViewModel
            {
                Name = usr.UserName,
                PhoneNumber = usr.PhoneNumber,
                Email = usr.Email,
                Role = _usrMngr.GetRolesAsync(usr).Result.FirstOrDefault() ?? "No role"
            }).ToList();

        [HttpGet("{email}")]
        public async Task<ActionResult<UserViewModel>> GetUser(string email)
        {
            if (email == null || _usrMngr.Users == null)
            {
                return NotFound();
            }
            var usrToGt = await _usrMngr.Users.FirstOrDefaultAsync(usr => usr.Email == email);
            if (usrToGt == null)
            {
                return NotFound();
            }
            var usrToRtrn = new UserViewModel
            {
                Name = usrToGt.UserName,
                PhoneNumber = usrToGt.PhoneNumber,
                Email = usrToGt.Email,
                Role = _usrMngr.GetRolesAsync(usrToGt).Result.FirstOrDefault() ?? "No role"
            };
            return usrToRtrn;
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
                IdentityResult[] idnttyRsltArr =
                {
                    await _usrMngr.CreateAsync(usrToAdd, user.Password),
                    await _usrMngr.AddToRoleAsync(usrToAdd, user.Role)
                };
                if (!(idnttyRsltArr[0].Succeeded && idnttyRsltArr[1].Succeeded))
                {
                    for (int i = 0; i < idnttyRsltArr.Length; i++)
                    {
                        foreach (var err in idnttyRsltArr[i].Errors)
                        {
                            ModelState.AddModelError(err.Code, err.Description);
                        }
                    }
                    return BadRequest(ModelState);
                }
                return Ok(user);
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        public async Task<ActionResult<UserViewModel>> Put(string email, UserViewModel user)
        {
            if (email != user.Email)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var usrToUpdte = await _usrMngr.Users.FirstOrDefaultAsync(usr => usr.Email == user.Email);
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

        [HttpDelete("{email}")]
        public async Task<ActionResult<UserViewModel>> Delete(string email)
        {
            if (_usrMngr.Users == null)
            {
                return BadRequest(new { err = "Entity set 'dbo.AspNetUsers'  is null."});
            }
            var usrToDlte = _usrMngr.Users.FirstOrDefault(usr => usr.Email == email);
            if (usrToDlte != null)
            {
                var dlteRslt = await _usrMngr.DeleteAsync(await _usrMngr.FindByEmailAsync(email));
                if (!dlteRslt.Succeeded)
                {
                    foreach (var err in dlteRslt.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                    }
                    return BadRequest(ModelState);
                }
            }
            return Ok();
        }

        private bool UserExists(string email)
            => _usrMngr.Users.Any(usr => usr.Email == email);
    }
}