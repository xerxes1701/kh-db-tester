using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB.Provider.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class BlobColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "Data",
                table: "WBinAblage",
                type: "varbinary(MAX)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "WBinAblageSplitted",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<byte[]>(type: "varbinary(8000)", nullable: true),
                    BlockNumber = table.Column<int>(type: "int", nullable: false)
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
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(MAX)",
                oldNullable: true);
        }
    }
}
