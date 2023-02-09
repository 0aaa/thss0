using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Thss0.Web.Config;
using Thss0.Web.Data;

namespace Thss0.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddAuthentication(optns =>
                            {
                                optns.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                                optns.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                            })
                            .AddJwtBearer(optns =>
                            {
                                optns.RequireHttpsMetadata = false;
                                optns.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidIssuer = AuthCredentials.ISSUER,
                                    ValidateAudience = true,
                                    ValidAudience = AuthCredentials.AUDIENCE,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    IssuerSigningKey = AuthCredentials.GetKey()
                                };
                            });
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();
            builder.Services.AddRazorPages();

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            policy =>
            {
                policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
            });
        });

            var app = builder.Build();

            app.UseSession();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseStatusCodePages("text/html", "404");
            app.UseHttpsRedirection();
            app.UseDefaultFiles();

            app.UseRouting();

            app.UseCors();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(cnfgrtn =>
            {
                cnfgrtn.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller=Home}/{action=Index}/{id?}");
                cnfgrtn.MapControllers();
            });
            app.MapRazorPages();

            app.Run();
        }
    }
}