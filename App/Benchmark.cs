using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[SimpleJob(RuntimeMoniker.Net80, launchCount: 2, warmupCount: 2, iterationCount: 5)]
public class MyBenchmark
{
    private IHost _app = null!;
    private byte[] _data = null!;

    [Params(100, 8000, 1_000_000)]
    public int DataLength;

    [Params(1, 10)]
    public int RecordCount;

    [Params(8000)]
    public int BlockLength;

    [GlobalSetup]
    public async Task Setup()
    {
        _app = Program.CreateHostBuilder([]).Build();
        _data = System.Text.Encoding.UTF8.GetBytes(new string('x', count: DataLength));

        Console.WriteLine($"Record count: {RecordCount}");
        Console.WriteLine($"Data length: {_data.Length}");
        Console.WriteLine($"Block length: {BlockLength}");

        await using var scope = _app.Services.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

        await dbContext.Database.MigrateAsync();

        await dbContext.WBinAblageUnlimited.ExecuteDeleteAsync();
        await dbContext.WBinAblageLimited.ExecuteDeleteAsync();
    }

    [Benchmark]
    public void InsertUnlimited()
    {
        using var scope = _app.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        for (var iRecord = 0; iRecord < RecordCount; ++iRecord)
        {
            WBinAblageUnlimited entity = new() { Data = _data };
            dbContext.WBinAblageUnlimited.Add(entity);
        }
        dbContext.SaveChanges();
    }

    [Benchmark]
    public void InsertLimited()
    {
        using var scope = _app.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

        for (var iRecord = 0; iRecord < RecordCount; ++iRecord)
        {
            var iBlock = 0;

            for (var offset = 0; offset < _data.Length; offset += BlockLength)
            {
                var block = _data[offset..(Math.Min(_data.Length, offset + BlockLength))];

                var record = new WBinAblageLimited()
                {
                    Data = block,
                    BlockNumber = iBlock++,
                };

                dbContext.WBinAblageLimited.Add(record);
            }
        }
        dbContext.SaveChanges();
    }
}
