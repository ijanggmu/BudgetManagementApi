using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class addgrosspremiumrsmdtthirdpartysuminsured : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BasicPremium",
                table: "PolicyDrafts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GovernmentSubsidyAmount",
                table: "PolicyDrafts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "GrossPremium",
                table: "PolicyDrafts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PayableAmount",
                table: "PolicyDrafts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PersonalAccidentPremium",
                table: "PolicyDrafts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RSMDTPremium",
                table: "PolicyDrafts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StampDuty",
                table: "PolicyDrafts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SumInsured",
                table: "PolicyDrafts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ThirdPartyPremium",
                table: "PolicyDrafts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPremium",
                table: "PolicyDrafts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatAmount",
                table: "PolicyDrafts",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasicPremium",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "GovernmentSubsidyAmount",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "GrossPremium",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "PayableAmount",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "PersonalAccidentPremium",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "RSMDTPremium",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "StampDuty",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "SumInsured",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "ThirdPartyPremium",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "TotalPremium",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "VatAmount",
                table: "PolicyDrafts");
        }
    }
}
