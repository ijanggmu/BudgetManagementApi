using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class customerusertablemodified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientClassification",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "FirstName_Np",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastName_Np",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MiddleName_Np",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Customers",
                newName: "FullNameNepali");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullNameNepali",
                table: "Customers",
                newName: "Phone");

            migrationBuilder.AddColumn<string>(
                name: "ClientClassification",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName_Np",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName_Np",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName_Np",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "Customers",
                type: "text",
                nullable: true);
        }
    }
}
