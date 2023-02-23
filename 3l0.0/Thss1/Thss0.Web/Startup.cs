using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Thss0.DAL.Config;
using Thss0.DAL.Context;

namespace Thss0.DAL
{
    internal class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration cnfgrtn)
            => Configuration = cnfgrtn;
        public void ConfigureServices(IServiceCollection srvcs)
        {
            srvcs.AddDbContext<Thss0Context>(optns
                => optns.UseSqlServer(Configuration.GetConnectionString("Thss0Connection")));
            srvcs.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<Thss0Context>();
            srvcs.AddAuthentication(optns
                => {
                    optns.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    optns.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(optns
                => {
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
            srvcs.AddControllersWithViews();
            srvcs.AddSession();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSession();
            if (env.IsDevelopment())
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
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpnts
                => {
                    endpnts.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpnts.MapControllers();
                });
        }
    }
}