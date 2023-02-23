using Caliburn.Micro;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Thss0.BLL.Services;
using Thss0.DAL.Context;
using Thss0.UI.Models;
using Thss0.UI.View;
using Thss0.UI.ViewModel;

namespace Thss0.UI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration cnfgrtn) => Configuration = cnfgrtn;

        public void ConfigureServices(IServiceCollection srvcs)
        {
            //srvcs.AddSingleton<IWindowManager, WindowManager>();
            //srvcs.AddSingleton<IEventAggregator, EventAggregator>();
            srvcs.AddDbContext<Thss0Context>(optns
                => optns.UseSqlServer(Configuration.GetConnectionString("Thss0Connection")));
            srvcs.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<Thss0Context>();
            srvcs.AddAuthentication(optns =>
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
                        ValidIssuer = AuthCredentialsService.GetIssuer(),
                        ValidateAudience = true,
                        ValidAudience = AuthCredentialsService.GetAudience(),
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = AuthCredentialsService.GetKey()
                    };
                });
            srvcs.AddControllersWithViews();
            srvcs.AddSession();
            srvcs.AddSingleton<ShellViewModel>();
            //srvcs.AddSingleton<ShellView>();
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
            app.UseEndpoints(endpnts =>
            {
                endpnts.MapControllerRoute(
                     name: "default",
                     pattern: "{controller=Home}/{action=Index}/{id?}/{purchase?}");
                endpnts.MapControllers();
            });
        }
    }
}
