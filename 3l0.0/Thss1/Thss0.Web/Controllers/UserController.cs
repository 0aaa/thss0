using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Thss0.DAL.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _usrMngr;
        private readonly RoleManager<IdentityRole> _rleMngr;
        public UserController(UserManager<IdentityUser> usrMngr, RoleManager<IdentityRole> rleMngr)
        {
            _usrMngr = usrMngr;
            _rleMngr = rleMngr;
        }
        // GET: HomeController
        public async Task<List<IdentityUser>> GetAll()
            => await _usrMngr.Users.ToListAsync();

        // GET: HomeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomeController/Create
        public async Task<IdentityUser> Create(string usrNme)
            => usrNme == null ? null : await _usrMngr.FindByNameAsync(usrNme);

        // POST: HomeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IdentityResult> Create(IdentityUser userToAdd, string usrPswrd)
        {
            if (usrPswrd != null)
            {
                userToAdd.PasswordHash = _usrMngr.PasswordHasher.HashPassword(userToAdd, usrPswrd);
                return await _usrMngr.CreateAsync(userToAdd);
            }
            return null;
        }

        // GET: HomeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IdentityResult> Edit(IdentityUser userToUpdate, string usrPswrd)
        {
            if (userToUpdate.Id != null)
            {
                userToUpdate.PasswordHash = usrPswrd == null ? userToUpdate.PasswordHash : _usrMngr.PasswordHasher.HashPassword(userToUpdate, usrPswrd);
                return await _usrMngr.UpdateAsync(userToUpdate);
            }
            return null;
        }

        // GET: HomeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IdentityResult> Delete(string usrNme)
            => await _usrMngr.DeleteAsync(_usrMngr.FindByNameAsync(usrNme).Result);
    }
}