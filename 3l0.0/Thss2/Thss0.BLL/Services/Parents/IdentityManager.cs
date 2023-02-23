using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Thss0.BLL.Services.Parents
{
    public class IdentityManager
    {
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public UserManager<IdentityUser> UserManager { get; set; }
        public IdentityManager(Thss0Context cntxt = null
            , UserManager<IdentityUser> um = null
            , IServiceProvider srvcePrvdr = null
            , ILogger<UserManager<IdentityUser>> lgrUm = null
            , SignInManager<IdentityUser> sm = null
            , ILogger<SignInManager<IdentityUser>> lgrSm = null
            , AuthenticationOptions authOp = null)
        {
            /*UserManager
                = new UserManager<IdentityUser>(new UserStore<IdentityUser>(cntxt)
                    , Options.Create(um.Options), um.PasswordHasher, um.UserValidators, um.PasswordValidators, um.KeyNormalizer, um.ErrorDescriber, srvcePrvdr
                    , srvcePrvdr.GetRequiredService<ILogger<UserManager<IdentityUser>>>());*/
            UserManager
                = new UserManager<IdentityUser>(new UserStore<IdentityUser>(cntxt)
                    , Options.Create(um.Options)
                    , um.PasswordHasher
                    , new List<UserValidator<IdentityUser>>()
                    , um.PasswordValidators
                    , um.KeyNormalizer
                    , um.ErrorDescriber
                    , srvcePrvdr
                    , srvcePrvdr.GetRequiredService<ILogger<UserManager<IdentityUser>>>());
            SignInManager
                = new SignInManager<IdentityUser>(UserManager
                    , new HttpContextAccessor()
                    , sm.ClaimsFactory
                    , Options.Create(sm.Options)
                    , srvcePrvdr.GetRequiredService<ILogger<SignInManager<IdentityUser>>>()
                    , new AuthenticationSchemeProvider(Options.Create(authOp))
                    , new DefaultUserConfirmation<IdentityUser>());
        }
    }
}