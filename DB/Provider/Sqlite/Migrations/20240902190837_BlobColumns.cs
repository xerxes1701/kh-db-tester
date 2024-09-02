using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dndemo.Migrations
{
    /// <inheritdoc />
    public partial class BlobColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "WBinAblage",
                type: "blob",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "WBinAblageSplitted",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<string>(type: "blob", nullable: true),
                    BlockNumber = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WBinAblageSplitted", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WBinAblageSplitted");

            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "WBinAblage",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "blob",
                oldNullable: true);
        }
    }
}
