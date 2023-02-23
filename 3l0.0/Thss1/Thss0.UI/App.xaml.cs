using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Windows;

namespace Thss0.UI
{
    public partial class App : Application
    {

        /*private void Application_Startup(object sender, StartupEventArgs e)
        {
            var hst = CreateHostBuilder().Build();
            using (IServiceScope scpe = hst.Services.CreateScope())
            {
                var services = scpe.ServiceProvider;
                try
                {
                    //TestData.Initialize(services.GetRequiredService<Thss0Context>());
                }
                catch (Exception excptn)
                {
                    services.GetRequiredService<ILogger<App>>()
                        .LogDebug(excptn, "Database seeding error");
                }
            }
            hst.Run();
        }
        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.UseStartup<Startup>());*/

        
    }
}