using Microsoft.EntityFrameworkCore;

public class MyDbContext
  : DbContext
{
    public MyDbContext(DbContextOptions options)
      : base(options)
    {
    }

    public required DbSet<WBinAblageUnlimited> WBinAblageUnlimited { get; init; }

    public required DbSet<WBinAblageLimited> WBinAblageLimited { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var typeNames = this.GetSpecialDbTypeNames();

        modelBuilder.Entity<WBinAblageUnlimited>(e =>
            {
                e.Property(e => e.Data).HasColumnType(typeNames.BlobUnlimited);
            });

        modelBuilder.Entity<WBinAblageLimited>(e =>
            {
                e.Property(e => e.Data).HasColumnType(typeNames.BlobLimited);
            });
    }
}
