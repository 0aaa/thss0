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
    public class AccountController(SignInManager<ApplicationUser> sm, RoleManager<IdentityRole> rm, ApplicationDbContext c) : Controller
    {
        private const string CLAIM_AUTHENTICATION_TYPE = "Token";
        private const string ERROR_TEXT = "Wrong login or password";

        private async Task<ClaimsIdentity> GetIdentity(string name, string pwd)
        {
            var sr = await sm.PasswordSignInAsync(name, pwd, false, false);
            var user = sm.UserManager.Users.FirstOrDefault(u => u.Name == name);
            if (sr.Succeeded && user != null)
            {
                return new ClaimsIdentity(
                    [new(ClaimsIdentity.DefaultNameClaimType, name)
                        , new(ClaimsIdentity.DefaultRoleClaimType, (await sm.UserManager.GetRolesAsync(user))[0])]
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
            if (!sm.UserManager.Users.Any())
            {
                await new TestData().Initialize(sm, rm, c);
            }
            var identity = await GetIdentity(user.Name, user.Password);
            if (identity.HasClaim(ClaimsIdentity.DefaultNameClaimType, user.Name))
            {
                var tkn = new JwtSecurityToken(
                    issuer: AuthCredentials.ISSUER
                    , audience: AuthCredentials.AUDIENCE
                    , notBefore: DateTime.Now
                    , claims: identity.Claims
                    , expires: DateTime.Now.Add(TimeSpan.FromDays(AuthCredentials.LIFETIME))
                    , signingCredentials: new SigningCredentials(AuthCredentials.GetSigningKey(), SecurityAlgorithms.HmacSha256)
                );
                var encodedTkn = new JwtSecurityTokenHandler().WriteToken(tkn);
                return Json(new
                {
                    access_token = encodedTkn
                    , username = identity.Name
                });
            }
            return BadRequest(ModelState);
        }
    }
}