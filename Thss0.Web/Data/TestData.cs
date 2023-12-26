using Microsoft.AspNetCore.Identity;
using Thss0.Web.Models.Entities;

namespace Thss0.Web.Data
{
    public class TestData
    {
        public async Task Initialize(SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            var userManager = signInManager.UserManager;
            var rolesAndNames = new[] { "admin", "professional", "client" };
            var passwords = new[] { "*1Admin", "*1Professional", "*1Client" };
            var procedures = new Procedure[rolesAndNames.Length * 2];
            ApplicationUser user;
            IQueryable<ApplicationUser> users;
            for (ushort i = 0; i < rolesAndNames.Length * 2; i++)
            {
                if (i < 3)
                {
                    roleManager.CreateAsync(new IdentityRole { Name = rolesAndNames[i] }).Wait();
                }
                user = new ApplicationUser
                {
                    Name = rolesAndNames[i / 2] + i % 2
                    , PhoneNumber = rolesAndNames[i / 2] + i % 2
                    , Email = $"{rolesAndNames[i / 2] + i % 2}@{rolesAndNames[i / 2] + i % 2}.com"
                    , Photo = File.ReadAllBytes($"{Directory.GetCurrentDirectory()}\\wwwroot\\img\\test_image_0.jpg")
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
                        , Name = DateTime.Now + (rolesAndNames[i / 2] != "admin" ? user.Name : "")
                        , ObtainmentTime = DateTime.Now
                        , Content = $"result{i}"
                    }
                    , Department = new Department
                    {
                        Id = Guid.NewGuid().ToString()
                        , Name = $"department{i}"
                    }
                };
                users = userManager.Users;
                for (ushort j = 0; j < users.Count(); j++)
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
            await context.Procedures.AddRangeAsync(procedures);
            await context.SaveChangesAsync();
        }
    }
}
