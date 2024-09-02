using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

[Command("test")]
public class TestCommand()
: ICommand
{
    public async ValueTask ExecuteAsync(IConsole console)
    {
        await Migrate(console);
        await TestBenchmarks(console);
        await PrintCount(console);
    }

    private static async Task TestBenchmarks(IConsole console)
    {
        console.Output.WriteLine("tesing benchmarks");

        var benchmark = new MyBenchmark()
        {
            DataLength = 1_000_000,
            BlockLength = 8000,
            RecordCount = 10,
        };

        await benchmark.Setup();
        benchmark.InsertLimited();
        benchmark.InsertUnlimited();
    }

    private static async Task Migrate(IConsole console)
    {
        console.Output.WriteLine("migrating...");

        var app = Program.CreateHostBuilder([]).Build();
        using var scope = app.Services.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

        await dbContext.Database.MigrateAsync();
    }

    private static async Task PrintCount(IConsole console)
    {
        var app = Program.CreateHostBuilder([]).Build();
        using var scope = app.Services.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

        var countLimited = await dbContext.WBinAblageLimited.CountAsync();
        console.Output.WriteLine($"count {nameof(MyDbContext.WBinAblageLimited)}: {countLimited}");

        var countUnlimited = await dbContext.WBinAblageUnlimited.CountAsync();
        console.Output.WriteLine($"count {nameof(MyDbContext.WBinAblageUnlimited)}: {countUnlimited}");
    }
}
