using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class MarineTariffe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarineTariffSchedules",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ProductCategory = table.Column<string>(type: "text", nullable: true),
                    ProductCategoryCode = table.Column<string>(type: "text", nullable: true),
                    Product = table.Column<string>(type: "text", nullable: true),
                    ProductCode = table.Column<string>(type: "text", nullable: true),
                    AllRiskValue = table.Column<decimal>(type: "numeric", nullable: false),
                    BasicRiskValue = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductDescription = table.Column<string>(type: "text", nullable: true),
                    MinimumRisk = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarineTariffSchedules", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarineTariffSchedules");
        }
    }
}
