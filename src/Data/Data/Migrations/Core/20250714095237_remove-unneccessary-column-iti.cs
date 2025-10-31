using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class removeunneccessarycolumniti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "ITI");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "ITI");

            migrationBuilder.DropColumn(
                name: "District",
                table: "ITI");

            migrationBuilder.DropColumn(
                name: "EffectiveDateBeforeEndorsement",
                table: "ITI");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "ITI");

            migrationBuilder.DropColumn(
                name: "ExpiryDateBeforeEndorsement",
                table: "ITI");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "ITI");

            migrationBuilder.DropColumn(
                name: "Municipality",
                table: "ITI");

            migrationBuilder.DropColumn(
                name: "Occupation",
                table: "ITI");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "ITI");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "ITI");

            migrationBuilder.DropColumn(
                name: "StreetAddress",
                table: "ITI");

            migrationBuilder.DropColumn(
                name: "Ward",
                table: "ITI");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "ITI",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "ITI",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "ITI",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveDateBeforeEndorsement",
                table: "ITI",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ITI",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDateBeforeEndorsement",
                table: "ITI",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "ITI",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Municipality",
                table: "ITI",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Occupation",
                table: "ITI",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "ITI",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "ITI",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                table: "ITI",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "ITI",
                type: "text",
                nullable: true);
        }
    }
}
