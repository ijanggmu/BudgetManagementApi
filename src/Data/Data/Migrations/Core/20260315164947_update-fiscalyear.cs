using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class updatefiscalyear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndYear",
                table: "NepaliFiscalYears");

            migrationBuilder.DropColumn(
                name: "StartYear",
                table: "NepaliFiscalYears");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateUtc",
                table: "NepaliFiscalYears",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDateUtc",
                table: "NepaliFiscalYears",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDateUtc",
                table: "NepaliFiscalYears");

            migrationBuilder.DropColumn(
                name: "StartDateUtc",
                table: "NepaliFiscalYears");

            migrationBuilder.AddColumn<int>(
                name: "EndYear",
                table: "NepaliFiscalYears",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartYear",
                table: "NepaliFiscalYears",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
