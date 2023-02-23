using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Thss0.DAL.Context;

namespace Thss0.BLL.Services.Parents
{
    public class IdentityManager
    {
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public IdentityManager()
        {
            var idnttyOptns = Options.Create(new IdentityOptions());
            var bldrPrvdr = new ServiceCollection().BuildServiceProvider();
            var usrMngr = new UserManager<IdentityUser>(new UserStore<IdentityUser>(new Thss0Context(new DbContextOptions<Thss0Context>()))
                                    , idnttyOptns
                                    , new PasswordHasher<IdentityUser>()
                                    , new List<UserValidator<IdentityUser>>()
                                    , new List<PasswordValidator<IdentityUser>>()
                                    , new UpperInvariantLookupNormalizer()
                                    , new IdentityErrorDescriber()
                                    , bldrPrvdr
                                    , bldrPrvdr.GetRequiredService<ILogger<UserManager<IdentityUser>>>());
            SignInManager = new SignInManager<IdentityUser>(usrMngr
                                    , new HttpContextAccessor()
                                    , new UserClaimsPrincipalFactory<IdentityUser>(usrMngr, Options.Create(new IdentityOptions()))
                                    , idnttyOptns
                                    , bldrPrvdr.GetRequiredService<ILogger<SignInManager<IdentityUser>>>()
                                    , new AuthenticationSchemeProvider(Options.Create(new AuthenticationOptions()))
                                    , new DefaultUserConfirmation<IdentityUser>());
        }
    }
}