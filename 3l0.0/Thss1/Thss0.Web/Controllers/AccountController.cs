using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Thss0.DAL.Context;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Thss0.DAL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _usrMngr;
        private readonly SignInManager<IdentityUser> _sgnInMngr;
        public AccountController(SignInManager<IdentityUser> sgnInMngr)
        {
            _usrMngr = sgnInMngr.UserManager;
            _sgnInMngr = sgnInMngr;
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IdentityResult> SignUp(Registration rgstrtn)
            => await _usrMngr.CreateAsync(new IdentityUser { Email = rgstrtn.Name, UserName = rgstrtn.Name }, rgstrtn.Password);
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<SignInResult> SignIn(SignIn sgnIn)
            => await _sgnInMngr.PasswordSignInAsync(sgnIn.Name, sgnIn.Password, false, false);
        [AutoValidateAntiforgeryToken]
        public async Task LogOut()
            => await _sgnInMngr.SignOutAsync();
    }
}
