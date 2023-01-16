using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thss0.Web.Models;

namespace Thss0.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _usrMngr;
        public UsersController(UserManager<IdentityUser> usrMngr)
        {
            _usrMngr = usrMngr;
        }

        // GET: Users
        public async Task<IActionResult> Index(string role)
            => View(_usrMngr.GetUsersInRoleAsync(role).Result.Select(usr => new User
            {
                Id = usr.Id,
                Name = usr.UserName,
                PhoneNumber = usr.PhoneNumber,
                Email = usr.Email,
                Role = _usrMngr.GetRolesAsync(usr).Result.FirstOrDefault() ?? "No role"
            }));

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _usrMngr.Users == null)
            {
                return NotFound();
            }
            var usrToRtrn = await _usrMngr.Users.FirstOrDefaultAsync(usr => usr.Id == id);
            if (usrToRtrn == null)
            {
                return NotFound();
            }
            return View(usrToRtrn);
        }

        // GET: Users/Create
        public IActionResult Create()
            => View();

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Password,PhoneNumber,Email,Role")] User user)
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
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _usrMngr.Users == null)
            {
                return NotFound();
            }
            var usrToRtrn = await _usrMngr.Users.FirstOrDefaultAsync(usr => usr.Id == id);
            if (usrToRtrn == null)
            {
                return NotFound();
            }
            return View(usrToRtrn);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Password,PhoneNumber,Email,Role")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var usrToUpdte = await _usrMngr.Users.FirstOrDefaultAsync(usr => usr.Id == user.Id);
                    if (usrToUpdte != null)
                    {
                        usrToUpdte.Id = user.Id;
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
                            return RedirectToAction(nameof(Edit), user);
                        }
                    }
                }
                catch (Exception)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _usrMngr.Users == null)
            {
                NotFound();
            }
            var usrToRtrn = await _usrMngr.Users.FirstOrDefaultAsync(usr => usr.Id == id);
            if (usrToRtrn == null)
            {
                NotFound();
            }
            return View(usrToRtrn);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_usrMngr.Users == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Users'  is null.");
            }
            var usrToDlte = _usrMngr.Users.FirstOrDefault(usr => usr.Id == id);
            if (usrToDlte != null)
            {
                var dlteRslt = await _usrMngr.DeleteAsync(await _usrMngr.FindByIdAsync(id));
                if (!dlteRslt.Succeeded)
                {
                    foreach (var err in dlteRslt.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                    }
                    return View();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
            => _usrMngr.Users.Any(usr => usr.Id == id);
    }
}
