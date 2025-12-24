using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class addbranchdesignationupdatetenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnderwriterDigitalSignatureUrl",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnderwriterName",
                table: "Tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerUserId",
                table: "Leads",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BranchId",
                table: "Fodos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DesignationId",
                table: "Fodos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeId",
                table: "Fodos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Fodos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PermanentDistrict",
                table: "Fodos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermanentMunicipality",
                table: "Fodos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermanentProvince",
                table: "Fodos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PermanentWard",
                table: "Fodos",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemporaryDistrict",
                table: "Fodos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemporaryMunicipality",
                table: "Fodos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemporaryProvince",
                table: "Fodos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TemporaryWard",
                table: "Fodos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BranchName = table.Column<string>(type: "text", nullable: true),
                    BranchCode = table.Column<string>(type: "text", nullable: true),
                    Province = table.Column<string>(type: "text", nullable: true),
                    District = table.Column<string>(type: "text", nullable: true),
                    Municipality = table.Column<string>(type: "text", nullable: true),
                    Ward = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Designations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Designations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PremiumCalculationConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PortfolioAlias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FiscalYear = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CalculationEngineType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_PremiumCalculationConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PremiumCalculationParameters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ConfigurationId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    ParameterKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ParameterName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DataType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    DefaultValue = table.Column<string>(type: "text", nullable: true),
                    MinValue = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    MaxValue = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_PremiumCalculationParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PremiumCalculationParameters_PremiumCalculationConfiguratio~",
                        column: x => x.ConfigurationId,
                        principalTable: "PremiumCalculationConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PremiumCalculationRateTables",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ConfigurationId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    TableName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SchemaJson = table.Column<string>(type: "text", nullable: false),
                    DataJson = table.Column<string>(type: "text", nullable: false),
                    LookupKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
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
                    table.PrimaryKey("PK_PremiumCalculationRateTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PremiumCalculationRateTables_PremiumCalculationConfiguratio~",
                        column: x => x.ConfigurationId,
                        principalTable: "PremiumCalculationConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PremiumCalculationRules",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ConfigurationId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    RuleName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RuleType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Expression = table.Column<string>(type: "text", nullable: true),
                    Condition = table.Column<string>(type: "text", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_PremiumCalculationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PremiumCalculationRules_PremiumCalculationConfigurations_Co~",
                        column: x => x.ConfigurationId,
                        principalTable: "PremiumCalculationConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fodos_BranchId",
                table: "Fodos",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Fodos_DesignationId",
                table: "Fodos",
                column: "DesignationId");

            migrationBuilder.CreateIndex(
                name: "IX_Fodos_EmployeeId",
                table: "Fodos",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_BranchCode",
                table: "Branches",
                column: "BranchCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Branches_BranchName_TenantId",
                table: "Branches",
                columns: new[] { "BranchName", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Designations_Title_TenantId",
                table: "Designations",
                columns: new[] { "Title", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PremiumCalculationConfig_Portfolio_FiscalYear_Active",
                table: "PremiumCalculationConfigurations",
                columns: new[] { "PortfolioAlias", "FiscalYear", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_PremiumCalculationConfig_Portfolio_FiscalYear_Dates",
                table: "PremiumCalculationConfigurations",
                columns: new[] { "PortfolioAlias", "FiscalYear", "EffectiveFrom", "EffectiveTo" });

            migrationBuilder.CreateIndex(
                name: "IX_PremiumCalculationParameter_ConfigId_Key",
                table: "PremiumCalculationParameters",
                columns: new[] { "ConfigurationId", "ParameterKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PremiumCalculationRateTable_ConfigId_TableName",
                table: "PremiumCalculationRateTables",
                columns: new[] { "ConfigurationId", "TableName" });

            migrationBuilder.CreateIndex(
                name: "IX_PremiumCalculationRule_ConfigId_Priority",
                table: "PremiumCalculationRules",
                columns: new[] { "ConfigurationId", "Priority" });

            migrationBuilder.AddForeignKey(
                name: "FK_Fodos_Branches_BranchId",
                table: "Fodos",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Fodos_Designations_DesignationId",
                table: "Fodos",
                column: "DesignationId",
                principalTable: "Designations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fodos_Branches_BranchId",
                table: "Fodos");

            migrationBuilder.DropForeignKey(
                name: "FK_Fodos_Designations_DesignationId",
                table: "Fodos");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Designations");

            migrationBuilder.DropTable(
                name: "PremiumCalculationParameters");

            migrationBuilder.DropTable(
                name: "PremiumCalculationRateTables");

            migrationBuilder.DropTable(
                name: "PremiumCalculationRules");

            migrationBuilder.DropTable(
                name: "PremiumCalculationConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_Fodos_BranchId",
                table: "Fodos");

            migrationBuilder.DropIndex(
                name: "IX_Fodos_DesignationId",
                table: "Fodos");

            migrationBuilder.DropIndex(
                name: "IX_Fodos_EmployeeId",
                table: "Fodos");

            migrationBuilder.DropColumn(
                name: "UnderwriterDigitalSignatureUrl",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "UnderwriterName",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Fodos");

            migrationBuilder.DropColumn(
                name: "DesignationId",
                table: "Fodos");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Fodos");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Fodos");

            migrationBuilder.DropColumn(
                name: "PermanentDistrict",
                table: "Fodos");

            migrationBuilder.DropColumn(
                name: "PermanentMunicipality",
                table: "Fodos");

            migrationBuilder.DropColumn(
                name: "PermanentProvince",
                table: "Fodos");

            migrationBuilder.DropColumn(
                name: "PermanentWard",
                table: "Fodos");

            migrationBuilder.DropColumn(
                name: "TemporaryDistrict",
                table: "Fodos");

            migrationBuilder.DropColumn(
                name: "TemporaryMunicipality",
                table: "Fodos");

            migrationBuilder.DropColumn(
                name: "TemporaryProvince",
                table: "Fodos");

            migrationBuilder.DropColumn(
                name: "TemporaryWard",
                table: "Fodos");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerUserId",
                table: "Leads",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
