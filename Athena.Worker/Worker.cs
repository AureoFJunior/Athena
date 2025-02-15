using Athena.Jobs;
using Hangfire;

namespace Athena.Worker;

public class Worker : BackgroundService
{
    private readonly IRecurringJobManager _recurringJobManager;

    public Worker(IRecurringJobManager recurringJobManager)
    {
        _recurringJobManager = recurringJobManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _recurringJobManager.AddOrUpdate<DataUpsertJob>(
        "DataUpsertJob",
        job => job.ExecuteAsync(),
        Cron.MinuteInterval(1));
    }
}