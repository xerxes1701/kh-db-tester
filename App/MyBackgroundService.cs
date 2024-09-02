using Microsoft.Extensions.Hosting;

class MyBackgroundService(IAppLifecycle appLifecycle)
  : BackgroundService
{
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await appLifecycle.WaitForStarted(stoppingToken);
    }
}


