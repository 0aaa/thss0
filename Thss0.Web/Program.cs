using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Thss0.Web.Config;
using Thss0.Web.Data;
using Thss0.Web.Models;

namespace Thss0.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                                options.UseLazyLoadingProxies()
                                        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")))
                            .AddDatabaseDeveloperPageExceptionFilter()
                            .AddIdentity<ApplicationUser, IdentityRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddAuthentication(options =>
                            {
                                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                            })
                            .AddJwtBearer(options =>
                            {
                                options.RequireHttpsMetadata = false;
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidIssuer = AuthCredentials.ISSUER,
                                    ValidateAudience = true,
                                    ValidAudience = AuthCredentials.AUDIENCE,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    IssuerSigningKey = AuthCredentials.GetSigningKey()
                                };
                            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();

            var app = builder.Build();

            app.UseSession();
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error")
            //        .UseHsts();
            //}
            app
                //.UseStatusCodePages("text/html", "404")
                .UseDefaultFiles()
                .UseStaticFiles()

                .UseCors(bldr =>
                    bldr.WithOrigins("http://localhost:3000")
                        .WithHeaders("content-type", "authorization")
                        .WithMethods("GET", "PUT", "POST", "DELETE", "OPTIONS"))
                .UseHttpsRedirection()
                .UseRouting()

                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(cnfgrtn =>
                {
                    cnfgrtn.MapControllerRoute(
                        name: "default",
                        pattern: "api/{controller=Home}/{action=Index}");
                    cnfgrtn.MapControllers();
                });
            //app.MapRazorPages();

            app.Run();
        }
    }
}