using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class addteantnewfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnderwriterName",
                table: "Tenants",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "UnderwriterDigitalSignatureUrl",
                table: "Tenants",
                newName: "PanNumber");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyStampUrl",
                table: "Tenants",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CompanyStampUrl",
                table: "Tenants");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Tenants",
                newName: "UnderwriterName");

            migrationBuilder.RenameColumn(
                name: "PanNumber",
                table: "Tenants",
                newName: "UnderwriterDigitalSignatureUrl");
        }
    }
}
