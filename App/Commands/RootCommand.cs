using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CliFx;
using CliFx.Infrastructure;
using CliFx.Attributes;

[Command]
public class RootCommand()
: ICommand
{
    public async ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine("RootCommand");

        var args = Environment.GetCommandLineArgs()[1..];

        var builder = Program.CreateHostBuilder(args);

        var app = builder.Build();

        var appLifecycle = (AppLifecycle)app.Services.GetRequiredService<IAppLifecycle>();

        await app.StartAsync();
        appLifecycle.SignalStarted();

        var dbFactory = new MyDbContextDesigntimeFactory();
        var dbContext = dbFactory.CreateDbContext(args);

        console.Output.WriteLine($"dbContext created: {dbContext is not null}");

        await app.WaitForShutdownAsync();
    }
}


