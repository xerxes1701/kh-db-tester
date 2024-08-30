using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CliFx;
using CliFx.Infrastructure;
using CliFx.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using Npgsql.EntityFrameworkCore.PostgreSQL;

await new CliApplicationBuilder()
  .AddCommand<RootCommand>()
  .Build()
  .RunAsync();

public static partial class Program
{
    public static IHostBuilder CreateHostBuilder(string[] args, bool isDesignTime)
    {
        var builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureHostConfiguration(builder =>
        {
            builder.AddEnvironmentVariables();
            builder.AddCommandLine(args);
        });

        builder.ConfigureServices((context, services) =>
        {
            if (!isDesignTime)
            {
                services.AddHostedService<MyBackgroundService>();
            }

            services.AddLogging();
            services.AddSingleton<IAppLifecycle, AppLifecycle>();
            services.AddDbContext<MyDbContext>((services, dbContextOptions) =>
            {
                var logger = services.GetService<ILogger>();
                var dbProvider = context.Configuration.GetRequiredSection("DB:Provider").Value;
                if (dbProvider is null)
                {
                    logger?.LogError("DB-Provider not specified, setting `DB:Provider` must be set");
                }

                Console.WriteLine($"dbProvider: {dbProvider}");
                logger?.LogInformation("DB-Provider: {dbProvider}", dbProvider);

                var connectionString = context.Configuration.GetConnectionString(dbProvider);
                Console.WriteLine($"connectionString: {connectionString}");

                _ = dbProvider.ToLowerInvariant() switch
                {
                    "sqlite" => dbContextOptions.UseSqlite(connectionString, sqliteDbContextOptions =>
                        sqliteDbContextOptions.MigrationsAssembly("DB.Provider.Sqlite")),

                    "oracle" => dbContextOptions.UseOracle(connectionString, oracleDbContextOptions =>
                        oracleDbContextOptions.MigrationsAssembly("DB.Provider.Oracle")),

                    "sqlserver" => dbContextOptions.UseSqlServer(connectionString, sqlServerDbContextOptions =>
                        sqlServerDbContextOptions.MigrationsAssembly("DB.Provider.SqlServer")),

                    "postgres" => dbContextOptions.UseNpgsql(connectionString, npgSqlDbContextOptions =>
                        npgSqlDbContextOptions.MigrationsAssembly("DB.Provider.Postgres")),

                    _ => throw new InvalidOperationException($"Unsupported DB provider: {dbProvider}")
                };
            });
        });

        builder.UseConsoleLifetime();

        return builder;
    }
}

[Command]
class RootCommand()
: ICommand
{
    public async ValueTask ExecuteAsync(IConsole console)
    {
        console.Output.WriteLine("RootCommand");

        var args = Environment.GetCommandLineArgs()[1..];

        var builder = Program.CreateHostBuilder(args, isDesignTime: false);

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

public class MyDbContextDesigntimeFactory
  : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        var builder = Program.CreateHostBuilder(args, isDesignTime: true);
        IHost host = builder.Build();
        var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        return dbContext;
    }
}

public interface IAppLifecycle
{
    Task WaitForStarted(CancellationToken cancellationToken);
}

class AppLifecycle
: IAppLifecycle
{
    private TaskCompletionSource _tcs = new();

    public void SignalStarted() => _tcs.TrySetResult();

    public Task WaitForStarted(CancellationToken cancellationToken)
    {
        cancellationToken.Register(() => _tcs.TrySetCanceled());
        return _tcs.Task;
    }
}

class MyBackgroundService(IAppLifecycle appLifecycle)
  : BackgroundService
{
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await appLifecycle.WaitForStarted(stoppingToken);
    }
}

