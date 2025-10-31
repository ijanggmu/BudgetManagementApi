using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class motorentitymodified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlueBookCopyImage",
                table: "Motors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NCDCerticficate",
                table: "Motors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoOfVechile",
                table: "Motors",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlueBookCopyImage",
                table: "Motors");

            migrationBuilder.DropColumn(
                name: "NCDCerticficate",
                table: "Motors");

            migrationBuilder.DropColumn(
                name: "PhotoOfVechile",
                table: "Motors");
        }
    }
}
