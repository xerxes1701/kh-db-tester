using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Design;

public class MyDbContextDesigntimeFactory
  : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        Console.WriteLine("creating db context...");
        var builder = Program.CreateHostBuilder(args);
        IHost host = builder.Build();
        var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        return dbContext;
    }
}


