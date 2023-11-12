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
                await Initialize();
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

        private async Task Initialize()
        {
            var userManager = _signInManager.UserManager;
            var rolesAndNames = new[] { "admin", "professional", "client" };
            var passwords = new[] { "*1Admin", "*1Professional", "*1Client" };
            var entities = new[] { "procedure", "result", "department" };
            var procedures = new Procedure[entities.Length * 2];
            ApplicationUser user;
            var users = new List<ApplicationUser>();
            for (ushort i = 0; i < rolesAndNames.Length * 2; i++)
            {
                if (i < 3)
                {
                    _roleManager.CreateAsync(new IdentityRole { Name = rolesAndNames[i] }).Wait();
                }
                user = new ApplicationUser
                {
                    Name = rolesAndNames[i / 2] + i % 2
                    , PhoneNumber = rolesAndNames[i / 2] + i % 2
                    , Email = $"{rolesAndNames[i / 2] + i % 2}@{rolesAndNames[i / 2] + i % 2}.com"
                    , Photo = System.IO.File.ReadAllBytes($"{Directory.GetCurrentDirectory()}\\wwwroot\\img\\test_image_0.jpg")
                };
                userManager.CreateAsync(user, passwords[i / 2])
                        .ContinueWith(delegate
                        {
                            userManager.AddToRoleAsync(user, rolesAndNames[i / 2]).Wait();
                        })
                        .Wait();

                procedures[i] = new Procedure
                {
                    Id = Guid.NewGuid().ToString()
                    , Name = $"procedure{i}"
                    , CreationTime = DateTime.Now
                    , BeginTime = new DateTime(2023, 11, 8, 12 + i, 0, 0)
                    , EndTime = new DateTime(2023, 11, 8, 12 + i, 15, 0)
                    , Result = new Result
                    {
                        Id = Guid.NewGuid().ToString()
                        , ObtainmentTime = DateTime.Now
                        , Content = $"result{i}"
                    }
                    , Department = new Department
                    {
                        Id = Guid.NewGuid().ToString()
                        , Name = $"department{i}"
                    }
                };
                users = userManager.Users.ToList();
                for (ushort j = 0; j < users.Count; j++)
                {                    
                    if (rolesAndNames[i / 2] != "admin")
                    {
                        procedures[0].User.Add(user);
                        procedures[i].Result?.User.Add(user);
                    }
                    if (rolesAndNames[i / 2] == "professional")
                    {
                        procedures[0].Department?.User.Add(user);
                    }
                }
            }
            await _context.Procedures.AddRangeAsync(procedures);
            await _context.SaveChangesAsync();
        }
    }
}