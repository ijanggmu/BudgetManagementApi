using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class quotationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InsuranceType",
                table: "Quotations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDirectBusiness",
                table: "Quotations",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PortfolioAlias",
                table: "Quotations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PremiumCalculationJson",
                table: "Quotations",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsuranceType",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "IsDirectBusiness",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "PortfolioAlias",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "PremiumCalculationJson",
                table: "Quotations");
        }
    }
}
