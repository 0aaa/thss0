using Thss0.DAL;
using Thss0.DAL.Context;

internal class Program
{
    private static void Main(string[] args)
    {
        var app = CreateHostBuilder(args).Build();
        using (IServiceScope scpe = app.Services.CreateScope())
        {
            var srvcePrvdr = scpe.ServiceProvider;
            try
            {
                TestData.Initialize(srvcePrvdr.GetRequiredService<Thss0Context>());
            }
            catch (Exception excptn)
            {
                srvcePrvdr.GetRequiredService<ILogger<Program>>()
                    .LogDebug(excptn, "Database seeding error");
            }
        }
        app.Run();
    }
    public static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(wbBldr
                => wbBldr.UseStartup<Startup>());
}