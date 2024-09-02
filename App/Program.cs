using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CliFx;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

Console.WriteLine("starting ...");
Console.WriteLine("  args:");
Console.WriteLine("  " + string.Join(' ', args));

await new CliApplicationBuilder()
  .AddCommandsFromThisAssembly()
  .Build()
  .RunAsync();

public static partial class Program
{
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        Console.WriteLine($"Creating {nameof(IHostBuilder)}...");

        var builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<IAppLifecycle, AppLifecycle>();

            services.AddDbContext<MyDbContext>((services, dbContextOptions) =>
            {
                var logger = services.GetRequiredService<ILogger<MyDbContext>>();
                var dbProvider = context.Configuration.GetRequiredSection("DB:Provider").Value switch
                {
                    "" or null => throw new InvalidOperationException("Setting `DB:Provider` not specified"),
                    string value => value,
                };

                logger.LogInformation("DB-Provider: {dbProvider}", dbProvider);

                var connectionString = context.Configuration.GetConnectionString(dbProvider) switch
                {
                    "" or null => throw new InvalidOperationException($"Setting `ConnectionStrings:{dbProvider}` not specified"),
                    string value => value,
                };

                logger.LogInformation("connection-string: {connectionString}", connectionString);

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
