using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class refactordatabasemigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OccupationJson",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "Occupation",
                table: "Customers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Occupation",
                table: "Customers");

            migrationBuilder.AddColumn<List<string>>(
                name: "OccupationJson",
                table: "Customers",
                type: "text[]",
                nullable: true);
        }
    }
}
