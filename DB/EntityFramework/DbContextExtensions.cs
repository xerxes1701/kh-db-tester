using Microsoft.EntityFrameworkCore;

public static class DbContextExtensions
{
    public static bool IsSqlServer(this DbContext context)
      => context.Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer";

    public static bool IsSqlite(this DbContext context)
      => context.Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite";

    public static SpecialDbDataTypeNames GetSpecialDbTypeNames(this DbContext context)
      => context.Database.ProviderName switch
      {
          "Microsoft.EntityFrameworkCore.SqlServer" => new(BlobUnlimited: "varbinary(MAX)", BlobLimited: "varbinary(8000)"),
          "Microsoft.EntityFrameworkCore.Sqlite" => new(BlobUnlimited: "blob", BlobLimited: "blob"),
          var provider =>
            throw new InvalidOperationException($"Unsupported DB provider: {provider}"),
      };
}

