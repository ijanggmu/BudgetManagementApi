using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class TravelConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HEOMITravelRates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Plan = table.Column<string>(type: "text", nullable: true),
                    PeriodFrom = table.Column<int>(type: "integer", nullable: false),
                    PeriodTo = table.Column<int>(type: "integer", nullable: false),
                    IndividualRate = table.Column<double>(type: "double precision", nullable: false),
                    FamilyRate = table.Column<double>(type: "double precision", nullable: false),
                    IsAnnualTrip = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HEOMITravelRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TravelUSDRates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Issuer = table.Column<string>(type: "text", nullable: true),
                    Group = table.Column<string>(type: "text", nullable: true),
                    PlanType = table.Column<string>(type: "text", nullable: true),
                    PeriodFrom = table.Column<int>(type: "integer", nullable: false),
                    PeriodTo = table.Column<int>(type: "integer", nullable: false),
                    IndividaulRate = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    FamilyRate = table.Column<decimal>(type: "numeric(15,2)", nullable: false),
                    DestintionIncludes = table.Column<string>(type: "text", nullable: true),
                    MultipleEntries = table.Column<string>(type: "text", nullable: true),
                    AgeFrom = table.Column<int>(type: "integer", nullable: false),
                    AgeTo = table.Column<int>(type: "integer", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelUSDRates", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HEOMITravelRates");

            migrationBuilder.DropTable(
                name: "TravelUSDRates");
        }
    }
}
