using Microsoft.EntityFrameworkCore;

public class MyDbContext
  : DbContext
{
    public MyDbContext(DbContextOptions options)
      : base(options)
    {
    }

    public required DbSet<WBinAblage> WBinAblage { get; init; }
}

public class WBinAblage
{
    public int Id { get; set; }
    public string? Data { get; set; }
}
