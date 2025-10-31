using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class adddocumentnumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentNumber",
                table: "PolicyDrafts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "PolicyDrafts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PolicyNumber",
                table: "PolicyDrafts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptNumber",
                table: "PolicyDrafts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentNumber",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "PolicyNumber",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "ReceiptNumber",
                table: "PolicyDrafts");
        }
    }
}
