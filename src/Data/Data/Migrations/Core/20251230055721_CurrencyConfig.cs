using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class CurrencyConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyExchangeRateConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BaseCurrency = table.Column<string>(type: "text", nullable: true),
                    TargetCurrency = table.Column<string>(type: "text", nullable: true),
                    BaseValue = table.Column<decimal>(type: "numeric", nullable: false),
                    TargetBuy = table.Column<decimal>(type: "numeric", nullable: false),
                    TargetSell = table.Column<decimal>(type: "numeric", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeRateConfigurations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyExchangeRateConfigurations");
        }
    }
}
