using Thss0.DAL.Context;

namespace Thss0.DAL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hst = CreateHostBuilder(args).Build();
            using (IServiceScope scpe = hst.Services.CreateScope())
            {
                var services = scpe.ServiceProvider;
                try
                {
                    TestData.Initialize(services.GetRequiredService<Thss0Context>());
                }
                catch (Exception excptn)
                {
                    services.GetRequiredService<ILogger<Program>>()
                        .LogDebug(excptn, "Database seeding error");
                }
            }
            hst.Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.UseStartup<Startup>());
    }
}