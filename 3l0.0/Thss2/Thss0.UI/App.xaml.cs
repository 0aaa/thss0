using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Windows;
using Thss0.BLL.Services;
using Thss0.UI.ViewModel;

namespace Thss0.UI
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var hst = CreateHostBuilder(e.Args).Build();
            using (IServiceScope scpe = hst.Services.CreateScope())
            {
                var srvcePrvdr = scpe.ServiceProvider;
                try
                {
                    var v = srvcePrvdr.GetRequiredService<SignInManager<IdentityUser>>();
                    var z = srvcePrvdr.GetRequiredService<UserManager<IdentityUser>>();
                    var vm = srvcePrvdr.GetRequiredService<ShellViewModel>();
                    vm.SignInManager = v;
                    vm.UserManager = z;
                    vm.ServiceProvider = srvcePrvdr;
                    vm.LoggerSm = srvcePrvdr.GetRequiredService<ILogger<SignInManager<IdentityUser>>>();
                    vm.LoggerUm = srvcePrvdr.GetRequiredService<ILogger<UserManager<IdentityUser>>>();
                    var ops = new AuthenticationOptions();
                    ops.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    ops.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    vm.AuthOptions = ops;
                    vm.Create();
                }
                catch (Exception excptn)
                {
                    srvcePrvdr.GetRequiredService<ILogger<App>>()
                                .LogDebug(excptn, "Database seeding error");
                }
            }
            hst.Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(wbBldr =>
                    wbBldr.UseStartup<Startup>());
    }
}