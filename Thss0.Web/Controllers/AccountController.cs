using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Thss0.Web.Models.ViewModels;

namespace Thss0.Web.Controllers
{
    public class AccountController : Controller
    {
        private const string ERROR_TEXT = "Wrong name or password";
        private readonly SignInManager<IdentityUser> _sgnInMngr;
        private readonly UserManager<IdentityUser> _usrMngr;
        public AccountController(SignInManager<IdentityUser> sgnInMngr, RoleManager<IdentityRole> rleMngr)
        {
            _sgnInMngr = sgnInMngr;
            _usrMngr = sgnInMngr.UserManager;
            if (!_usrMngr.Users.Any())
            {
                string[] usrAndRleNms = { "admin", "user" };
                string[] usrPswrds = { "*1Admin", "*1UserUser" };
                for (int i = 0; i < usrAndRleNms.Length; i++)
                {
                    rleMngr.CreateAsync(new IdentityRole { Name = usrAndRleNms[i] }).Wait();
                    _usrMngr.CreateAsync(new IdentityUser
                        {
                            UserName = usrAndRleNms[i],
                            PhoneNumber = usrAndRleNms[i],
                            Email = usrAndRleNms[i] + '@' + usrAndRleNms[i] + ".com"
                        }, usrPswrds[i])
                    .ContinueWith(async delegate
                        {
                            await _usrMngr.AddToRoleAsync(_usrMngr.FindByNameAsync(usrAndRleNms[i]).Result, usrAndRleNms[i]);
                        })
                    .Wait();
                }
            }
        }

        public IActionResult Register()
            => View();

        public IActionResult Login(string rtrnURL)
            => View(new LoginViewModel { ReturnURL = rtrnURL });

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel rgstrVwMdl)
        {
            if (ModelState.IsValid)
            {
                var usrToRgstr = new IdentityUser { Email = rgstrVwMdl.Name, UserName = rgstrVwMdl.Name };
                var crteRslt = await _usrMngr.CreateAsync(usrToRgstr, rgstrVwMdl.Password);
                if (crteRslt.Succeeded)
                {
                    await _sgnInMngr.SignInAsync(usrToRgstr, false);
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var err in crteRslt.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                    }
                }
            }
            return View(rgstrVwMdl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel lgnVwMdl)
        {
            if (ModelState.IsValid)
            {
                var lgnRslt = await _sgnInMngr.PasswordSignInAsync(lgnVwMdl.Name, lgnVwMdl.Password, false, false);
                if (lgnRslt.Succeeded)
                {
                    if (!string.IsNullOrEmpty(lgnVwMdl.ReturnURL) && Url.IsLocalUrl(lgnVwMdl.ReturnURL))
                    {
                        return Redirect(lgnVwMdl.ReturnURL);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", ERROR_TEXT);
                }
            }
            return View(lgnVwMdl);
        }

        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _sgnInMngr.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}