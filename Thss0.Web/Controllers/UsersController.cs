using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Models.ViewModels;

namespace Thss0.Web.Controllers
{
    //[Authorize(Roles ="admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _usrMngr;
        private readonly RoleManager<IdentityRole> _rleMngr;
        public UsersController(UserManager<IdentityUser> usrMngr, RoleManager<IdentityRole> rleMngr)
        {
            _usrMngr = usrMngr;
            _rleMngr = rleMngr;
        }

        public async Task<IActionResult> Index(string role)
        {
            ViewData["role"] = role;
            return View(_usrMngr.GetUsersInRoleAsync(role).Result.Select(usr => new UserViewModel
            {
                Name = usr.UserName,
                PhoneNumber = usr.PhoneNumber,
                Email = usr.Email,
                Role = _usrMngr.GetRolesAsync(usr).Result.FirstOrDefault() ?? "No role"
            }));
        }

        public async Task<IActionResult> Details(string email)
        {
            if (email == null || _usrMngr.Users == null)
            {
                return NotFound();
            }
            var usrToRtrn = await _usrMngr.FindByEmailAsync(email);
            if (usrToRtrn == null)
            {
                return NotFound();
            }
            return View(new UserViewModel
            {
                Name = usrToRtrn.UserName,
                PhoneNumber = usrToRtrn.PhoneNumber,
                Email = usrToRtrn.Email,
                Role = _usrMngr.GetRolesAsync(usrToRtrn).Result.FirstOrDefault() ?? "No role"
            });
        }

        public IActionResult Create(string role)
        {
            ViewData["role"] = role;
            ViewData["roles"] = new SelectList(_rleMngr.Roles.Select(rle => rle.Name));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Password,PhoneNumber,Email,Role")] UserViewModel user)
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
                    return RedirectToAction(nameof(Create), user);
                }
                return RedirectToAction(nameof(Index), new RouteValueDictionary { { "role", user.Role } });
            }
            return View(user);
        }

        public async Task<IActionResult> Edit(string email)
        {
            if (email == null || _usrMngr.Users == null)
            {
                return NotFound();
            }
            var usrToRtrn = await _usrMngr.FindByEmailAsync(email);
            if (usrToRtrn == null)
            {
                return NotFound();
            }
            ViewData["roles"] = new SelectList(_rleMngr.Roles.Select(rle => rle.Name));
            return View(new UserViewModel
            {
                Name = usrToRtrn.UserName,
                PhoneNumber = usrToRtrn.PhoneNumber,
                Email = usrToRtrn.Email,
                Role = _usrMngr.GetRolesAsync(usrToRtrn).Result.FirstOrDefault() ?? "No role"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string email, [Bind("Name,Password,PhoneNumber,Email,Role")] UserViewModel user)
        {
            if (email != user.Email)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var usrToUpdte = await _usrMngr.FindByEmailAsync(user.Email);
                    if (usrToUpdte != null)
                    {
                        usrToUpdte.UserName = user.Name;
                        usrToUpdte.PasswordHash = _usrMngr.PasswordHasher.HashPassword(usrToUpdte, user.Password);
                        usrToUpdte.PhoneNumber = user.PhoneNumber;
                        usrToUpdte.Email = user.Email;
                        var idnttyRslt = await _usrMngr.UpdateAsync(usrToUpdte);
                        await _usrMngr.RemoveFromRolesAsync(usrToUpdte, await _usrMngr.GetRolesAsync(usrToUpdte));
                        await _usrMngr.AddToRoleAsync(usrToUpdte, user.Role);
                        if (!idnttyRslt.Succeeded)
                        {
                            foreach (var err in idnttyRslt.Errors)
                            {
                                ModelState.AddModelError(err.Code, err.Description);
                            }
                            return RedirectToAction(nameof(Edit), user);
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
                return RedirectToAction(nameof(Index), new RouteValueDictionary { { "role", user.Role } });
            }
            return View(user);
        }

        public async Task<IActionResult> Remove(string email)
        {
            if (email == null || _usrMngr.Users == null)
            {
                NotFound();
            }
            var usrToRtrn = await _usrMngr.FindByEmailAsync(email);
            if (usrToRtrn == null)
            {
                NotFound();
            }
            return View(new UserViewModel
            {
                Name = usrToRtrn.UserName,
                PhoneNumber = usrToRtrn.PhoneNumber,
                Email = usrToRtrn.Email,
                Role = _usrMngr.GetRolesAsync(usrToRtrn).Result.FirstOrDefault() ?? "No role"
            });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string email)
        {
            if (_usrMngr.Users == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Users'  is null.");
            }
            var usrToDlte = await _usrMngr.FindByEmailAsync(email);
            if (usrToDlte != null)
            {
                var dlteRslt = await _usrMngr.DeleteAsync(usrToDlte);
                if (!dlteRslt.Succeeded)
                {
                    foreach (var err in dlteRslt.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                    }
                    return View();
                }
            }
            return RedirectToAction(nameof(Index), new RouteValueDictionary { { "roleName", (_usrMngr.GetRolesAsync(usrToDlte)).Result.FirstOrDefault() } });
        }

        private bool UserExists(string email)
            => _usrMngr.Users.Any(usr => usr.Email == email);
    }
}
