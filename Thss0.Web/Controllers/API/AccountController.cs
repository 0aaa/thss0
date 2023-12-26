using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Thss0.Web.Config;
using Thss0.Web.Data;
using Thss0.Web.Models.ViewModels;
using Thss0.Web.Models.Entities;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : Controller
    {
        private const string CLAIM_AUTHENTICATION_TYPE = "Token";
        private const string ERROR_TEXT = "Wrong login or password";
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        private async Task<ClaimsIdentity> GetIdentity(string name, string password)
        {
            var result = await _signInManager.PasswordSignInAsync(name, password, false, false);
            var user = _signInManager.UserManager.Users.FirstOrDefault(u => u.Name == name);
            if (result.Succeeded && user != null)
            {
                return new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, name)
                        , new Claim(ClaimsIdentity.DefaultRoleClaimType, (await _signInManager.UserManager.GetRolesAsync(user))[0])
                    }
                    , CLAIM_AUTHENTICATION_TYPE
                    , ClaimsIdentity.DefaultNameClaimType
                    , ClaimsIdentity.DefaultRoleClaimType);
            }
            ModelState.AddModelError("400", ERROR_TEXT);
            return new ClaimsIdentity();
        }

        [HttpPost]
        public async Task<ActionResult> Token(UserViewModel user)
        {
            if (!_signInManager.UserManager.Users.Any())
            {
                await new TestData().Initialize(_signInManager, _roleManager, _context);
            }
            var identity = await GetIdentity(user.Name, user.Password);
            if (identity.HasClaim(ClaimsIdentity.DefaultNameClaimType, user.Name))
            {
                var token = new JwtSecurityToken(
                    issuer: AuthCredentials.ISSUER
                    , audience: AuthCredentials.AUDIENCE
                    , notBefore: DateTime.Now
                    , claims: identity.Claims
                    , expires: DateTime.Now.Add(TimeSpan.FromDays(AuthCredentials.LIFETIME))
                    , signingCredentials: new SigningCredentials(AuthCredentials.GetSigningKey(), SecurityAlgorithms.HmacSha256)
                );
                var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
                return Json(new
                {
                    access_token = encodedToken
                    , username = identity.Name
                });
            }
            return BadRequest(ModelState);
        }
    }
}