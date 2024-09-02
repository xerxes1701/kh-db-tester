using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dndemo.Migrations
{
    /// <inheritdoc />
    public partial class BlobColumns2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WBinAblage");

            migrationBuilder.DropTable(
                name: "WBinAblageSplitted");

            migrationBuilder.CreateTable(
                name: "WBinAblageLimited",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<byte[]>(type: "blob", nullable: true),
                    BlockNumber = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WBinAblageLimited", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WBinAblageUnlimited",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<byte[]>(type: "blob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WBinAblageUnlimited", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WBinAblageLimited");

            migrationBuilder.DropTable(
                name: "WBinAblageUnlimited");

            migrationBuilder.CreateTable(
                name: "WBinAblage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<string>(type: "blob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WBinAblage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WBinAblageSplitted",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BlockNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<string>(type: "blob", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WBinAblageSplitted", x => x.Id);
                });
        }
    }
}
