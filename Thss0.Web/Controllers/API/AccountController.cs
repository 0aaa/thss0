using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Thss0.Web.Config;
using Thss0.Web.Models.ViewModels;

namespace Thss0.Web.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : Controller
    {
        private const string DEFAULT_ROLE_CLAIM_TYPE = "admin";
        private const string CLAIM_AUTHENTICATION_TYPE = "Token";
        private const string ERROR_TEXT = "Wrong login or password";
        private readonly SignInManager<IdentityUser> _sgnInMngr;
        public AccountController(SignInManager<IdentityUser> sgnInMngr)
            => _sgnInMngr = sgnInMngr;

        private async Task<ClaimsIdentity> GetIdentity(string nme, string pswrd)
        {
            var sgnInRslt = await _sgnInMngr.PasswordSignInAsync(nme, pswrd, false, false);
            if (sgnInRslt.Succeeded)
            {
                return new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, nme),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, DEFAULT_ROLE_CLAIM_TYPE)
                    }
                    , CLAIM_AUTHENTICATION_TYPE
                    , ClaimsIdentity.DefaultNameClaimType
                    , ClaimsIdentity.DefaultRoleClaimType);
            }
            ModelState.AddModelError("", ERROR_TEXT);
            return new ClaimsIdentity();
        }

        [HttpPost]
        public async Task<IActionResult> Token(UserViewModel usr)
        {
            var idntty = await GetIdentity(usr.Name, usr.Password);
            if (idntty.HasClaim(ClaimsIdentity.DefaultNameClaimType, usr.Name))
            {
                var tkn = new JwtSecurityToken(
                    issuer: AuthCredentials.ISSUER,
                    audience: AuthCredentials.AUDIENCE,
                    notBefore: DateTime.Now,
                    claims: idntty.Claims,
                    expires: DateTime.Now.Add(TimeSpan.FromMinutes(AuthCredentials.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthCredentials.GetKey(), SecurityAlgorithms.HmacSha256)
                    );
                var encodedTkn = new JwtSecurityTokenHandler().WriteToken(tkn);
                return Json(new
                {
                    access_token = encodedTkn,
                    username = idntty.Name
                });
            }
            return BadRequest(new { err = ERROR_TEXT });
        }
    }
}