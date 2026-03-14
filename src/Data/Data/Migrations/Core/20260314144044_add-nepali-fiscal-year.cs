using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class addnepalifiscalyear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FiscalYearId",
                table: "Budgets",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Unit",
                table: "Budgets",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitAmount",
                table: "Budgets",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "NepaliFiscalYears",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    StartYear = table.Column<int>(type: "integer", nullable: false),
                    EndYear = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NepaliFiscalYears", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_FiscalYearId",
                table: "Budgets",
                column: "FiscalYearId");

            migrationBuilder.CreateIndex(
                name: "IX_NepaliFiscalYears_Code_TenantId",
                table: "NepaliFiscalYears",
                columns: new[] { "Code", "TenantId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_NepaliFiscalYears_FiscalYearId",
                table: "Budgets",
                column: "FiscalYearId",
                principalTable: "NepaliFiscalYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_NepaliFiscalYears_FiscalYearId",
                table: "Budgets");

            migrationBuilder.DropTable(
                name: "NepaliFiscalYears");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_FiscalYearId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "FiscalYearId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "UnitAmount",
                table: "Budgets");
        }
    }
}
