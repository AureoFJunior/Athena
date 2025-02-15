using Athena.Domain.Repositories;
using Athena.Infrastructure.Data;
using Athena.Infrastructure.Repositories;
using Athena.Jobs;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Athena.Worker;
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                services.AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(hostContext.Configuration.GetConnectionString("HangfireConnection")));

                services.AddHangfireServer();

                services.AddScoped<IDataEntryRepository, DataEntryRepository>();
                services.AddHttpClient();
                services.AddScoped<DataUpsertJob>();
                services.AddHostedService<Worker>();
            });
}
