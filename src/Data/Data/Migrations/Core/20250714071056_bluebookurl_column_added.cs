using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class bluebookurl_column_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "YearsFromRegistrationDateYears",
                table: "Motors",
                type: "text",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "BlueBookCopyImageUrl",
                table: "Motors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleType",
                table: "Motors",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlueBookCopyImageUrl",
                table: "Motors");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "Motors");

            migrationBuilder.AlterColumn<DateTime>(
                name: "YearsFromRegistrationDateYears",
                table: "Motors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
