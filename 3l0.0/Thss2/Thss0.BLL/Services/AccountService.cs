using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Thss0.BLL.DTOs;
using Thss0.BLL.Services.Parents;

namespace Thss0.BLL.Services
{
    public class AccountService : IdentityManager
    {
        public AccountService(Thss0Context cntxt = null
                , UserManager<IdentityUser> um = null
                , IServiceProvider srvcePrvdr = null
                , ILogger<UserManager<IdentityUser>> lgrUm = null
                , SignInManager<IdentityUser> sm = null
                , ILogger<SignInManager<IdentityUser>> lgrSm = null
                , AuthenticationOptions authOp = null)
            : base(cntxt, um, srvcePrvdr, lgrUm, sm, lgrSm, authOp)
        {
            string testValue = "5";
            //SignUp(new UserDTO { Name = "test"+testValue, Password = "TestTest0*", PhoneNumber = "0123456789", Email = testValue+"test@gmail.com" });
            SignIn(new UserDTO { Name = "test5", Password = "TestTest0*", PhoneNumber = "0123456789", Email = testValue + "test@gmail.com" });
        }
        public Task<IdentityResult> SignUp(UserDTO userToSignUp)
        {
            var userToAdd = new IdentityUser
            {
                UserName = userToSignUp.Name,
                PhoneNumber = userToSignUp.PhoneNumber,
                Email = userToSignUp.Email
            };
            var res = UserManager.CreateAsync(userToAdd, userToSignUp.Password);
            SignInManager.UserManager.AddToRoleAsync(userToAdd, userToSignUp.Role);
            return res;
        }
        public async Task<SignInResult> SignIn(UserDTO userToSignIn)
        {
            SignInResult res = null;
            try
            {
                res = SignInManager.CheckPasswordSignInAsync(await UserManager.FindByNameAsync(userToSignIn.Name), userToSignIn.Password, true).Result;
            }
            catch (Exception excptn)
            {
                Console.WriteLine(excptn.Message);
            }
            return res;
        }
        public async Task LogOut()
            => await SignInManager.SignOutAsync();
    }
}