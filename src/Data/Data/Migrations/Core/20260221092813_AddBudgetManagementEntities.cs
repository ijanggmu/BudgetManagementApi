using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class AddBudgetManagementEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalculationConfigurations");

            migrationBuilder.DropTable(
                name: "Corporates");

            migrationBuilder.DropTable(
                name: "Fodos");

            migrationBuilder.DropTable(
                name: "GlobalConfigurations");

            migrationBuilder.DropTable(
                name: "ITIFamilyMembers");

            migrationBuilder.DropTable(
                name: "LeadActivities");

            migrationBuilder.DropTable(
                name: "MarineTariffSchedules");

            migrationBuilder.DropTable(
                name: "PolicyDrafts");

            migrationBuilder.DropTable(
                name: "PremiumCalculationParameters");

            migrationBuilder.DropTable(
                name: "PremiumCalculationRateTables");

            migrationBuilder.DropTable(
                name: "PremiumCalculationRules");

            migrationBuilder.DropTable(
                name: "PropertyRiskConfigurations");

            migrationBuilder.DropTable(
                name: "PropertySubsidySILimits");

            migrationBuilder.DropTable(
                name: "QuotationItems");

            migrationBuilder.DropTable(
                name: "RenewalReminders");

            migrationBuilder.DropTable(
                name: "ITI");

            migrationBuilder.DropTable(
                name: "Motors");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "PrivateVehicles");

            migrationBuilder.DropTable(
                name: "PremiumCalculationConfigurations");

            migrationBuilder.DropTable(
                name: "Quotations");

            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSignatures",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    SignatureUrl = table.Column<string>(type: "text", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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
                    table.PrimaryKey("PK_UserSignatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalConfigs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DepartmentId = table.Column<string>(type: "text", nullable: true),
                    StepsJson = table.Column<string>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_ApprovalConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalConfigs_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BudgetRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DepartmentId = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    NextApproverRoleId = table.Column<string>(type: "text", nullable: true),
                    CurrentApprovalStep = table.Column<int>(type: "integer", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MemoFileUrl = table.Column<string>(type: "text", nullable: true),
                    ApprovalHistoryJson = table.Column<string>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_BudgetRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetRequests_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DepartmentId = table.Column<string>(type: "text", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Quarter = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    AllocatedAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "numeric", nullable: false),
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
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budgets_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Memos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BudgetRequestId = table.Column<string>(type: "text", nullable: true),
                    RequestedBy = table.Column<string>(type: "text", nullable: true),
                    RequestedByDepartment = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: true),
                    Department = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: true),
                    ApproversJson = table.Column<string>(type: "jsonb", nullable: true),
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
                    table.PrimaryKey("PK_Memos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Memos_BudgetRequests_BudgetRequestId",
                        column: x => x.BudgetRequestId,
                        principalTable: "BudgetRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalConfigs_DepartmentId_TenantId",
                table: "ApprovalConfigs",
                columns: new[] { "DepartmentId", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRequests_DepartmentId",
                table: "BudgetRequests",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetRequests_TenantId_RequestedDate",
                table: "BudgetRequests",
                columns: new[] { "TenantId", "RequestedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_DepartmentId_Year_Quarter_TenantId",
                table: "Budgets",
                columns: new[] { "DepartmentId", "Year", "Quarter", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name_TenantId",
                table: "Departments",
                columns: new[] { "Name", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Memos_BudgetRequestId_TenantId",
                table: "Memos",
                columns: new[] { "BudgetRequestId", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSignatures_UserId_TenantId",
                table: "UserSignatures",
                columns: new[] { "UserId", "TenantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalConfigs");

            migrationBuilder.DropTable(
                name: "Budgets");

            migrationBuilder.DropTable(
                name: "Memos");

            migrationBuilder.DropTable(
                name: "UserSignatures");

            migrationBuilder.DropTable(
                name: "BudgetRequests");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.CreateTable(
                name: "CalculationConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "integer", nullable: false),
                    ApprovedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataType = table.Column<string>(type: "text", nullable: true),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Level = table.Column<string>(type: "text", nullable: true),
                    LowerLimit = table.Column<decimal>(type: "numeric", nullable: false),
                    LowerLimitEquals = table.Column<bool>(type: "boolean", nullable: false),
                    PortfolioAlias = table.Column<string>(type: "text", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    TypeEnumValue = table.Column<int>(type: "integer", nullable: true),
                    UpperLimit = table.Column<decimal>(type: "numeric", nullable: false),
                    UpperLimitEquals = table.Column<bool>(type: "boolean", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(15,4)", nullable: false),
                    ValueType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Corporates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    CorporateName = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corporates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Corporates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Fodos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BranchId = table.Column<string>(type: "text", nullable: true),
                    DesignationId = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmployeeId = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PermanentDistrict = table.Column<string>(type: "text", nullable: true),
                    PermanentMunicipality = table.Column<string>(type: "text", nullable: true),
                    PermanentProvince = table.Column<string>(type: "text", nullable: true),
                    PermanentWard = table.Column<int>(type: "integer", nullable: true),
                    TemporaryDistrict = table.Column<string>(type: "text", nullable: true),
                    TemporaryMunicipality = table.Column<string>(type: "text", nullable: true),
                    TemporaryProvince = table.Column<string>(type: "text", nullable: true),
                    TemporaryWard = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fodos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fodos_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fodos_Designations_DesignationId",
                        column: x => x.DesignationId,
                        principalTable: "Designations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fodos_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GlobalConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataType = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Level = table.Column<string>(type: "text", nullable: true),
                    LowerLimit = table.Column<decimal>(type: "numeric", nullable: false),
                    LowerLimitEquals = table.Column<bool>(type: "boolean", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    TypeEnumValue = table.Column<int>(type: "integer", nullable: true),
                    UpperLimit = table.Column<decimal>(type: "numeric", nullable: false),
                    UpperLimitEquals = table.Column<bool>(type: "boolean", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(15,4)", nullable: false),
                    ValueType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ITI",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmergencyContactName = table.Column<string>(type: "text", nullable: true),
                    EmergencyContactNumber = table.Column<string>(type: "text", nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "numeric", nullable: false),
                    FatherHusbandName = table.Column<string>(type: "text", nullable: true),
                    IdType = table.Column<string>(type: "text", nullable: true),
                    InsuranceType = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PassportNumber = table.Column<string>(type: "text", nullable: true),
                    PlanType = table.Column<string>(type: "text", nullable: false),
                    PolicyPeriodInDays = table.Column<int>(type: "integer", nullable: false),
                    PremiumAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TravellingCountry = table.Column<string>(type: "text", nullable: true),
                    TripType = table.Column<string>(type: "text", nullable: false),
                    TypeOfInsured = table.Column<string>(type: "text", nullable: true),
                    VisitingCountry = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITI", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadActivities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Kind = table.Column<string>(type: "text", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LeadId = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    When = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ProspectId = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeadLineDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstimatedPremium = table.Column<decimal>(type: "numeric", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OwnerUserId = table.Column<string>(type: "text", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leads_Prospects_ProspectId",
                        column: x => x.ProspectId,
                        principalTable: "Prospects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarineTariffSchedules",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AllRiskValue = table.Column<decimal>(type: "numeric", nullable: false),
                    BasicRiskValue = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MinimumRisk = table.Column<decimal>(type: "numeric", nullable: false),
                    Product = table.Column<string>(type: "text", nullable: true),
                    ProductCategory = table.Column<string>(type: "text", nullable: true),
                    ProductCategoryCode = table.Column<string>(type: "text", nullable: true),
                    ProductCode = table.Column<string>(type: "text", nullable: true),
                    ProductDescription = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarineTariffSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Motors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AgeOfVehicle = table.Column<int>(type: "integer", nullable: true),
                    BlueBookCopyImage = table.Column<string>(type: "text", nullable: true),
                    BlueBookCopyImageUrl = table.Column<string>(type: "text", nullable: true),
                    ChasisNumber = table.Column<string>(type: "text", nullable: true),
                    CompulsoryExcess = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CubicCapacity = table.Column<decimal>(type: "numeric", nullable: true),
                    CurrentMarketPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    DateOfPurchase = table.Column<string>(type: "text", nullable: true),
                    Days = table.Column<int>(type: "integer", nullable: false),
                    EngineNumber = table.Column<string>(type: "text", nullable: true),
                    Financer = table.Column<string>(type: "text", nullable: true),
                    IsComprehensive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsThirdParty = table.Column<bool>(type: "boolean", nullable: false),
                    KilloWatt = table.Column<decimal>(type: "numeric", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Maintenance = table.Column<bool>(type: "boolean", nullable: true),
                    ManufactureCompany = table.Column<string>(type: "text", nullable: true),
                    ManufactureYear = table.Column<string>(type: "text", nullable: true),
                    Model = table.Column<string>(type: "text", nullable: true),
                    NCDCerticficate = table.Column<string>(type: "text", nullable: true),
                    NCDYears = table.Column<int>(type: "integer", nullable: false),
                    NumberofSeatsIncludingDriver = table.Column<int>(type: "integer", nullable: false),
                    ParkingGarageOpen = table.Column<bool>(type: "boolean", nullable: true),
                    ParkingPlaceGarage = table.Column<bool>(type: "boolean", nullable: true),
                    PartyId = table.Column<string>(type: "text", nullable: true),
                    PhotoOfVechile = table.Column<string>(type: "text", nullable: true),
                    PurchasedNewOld = table.Column<bool>(type: "boolean", nullable: false),
                    RateOfDepreciation = table.Column<decimal>(type: "numeric", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: true),
                    RiotStrike = table.Column<bool>(type: "boolean", nullable: false),
                    SubModel = table.Column<string>(type: "text", nullable: true),
                    TotalExcess = table.Column<decimal>(type: "numeric", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    ValueOfAccessories = table.Column<decimal>(type: "numeric", nullable: true),
                    VehicleForHireOrReward = table.Column<bool>(type: "boolean", nullable: true),
                    VehicleType = table.Column<int>(type: "integer", nullable: false),
                    VoluntaryExcess = table.Column<decimal>(type: "numeric", nullable: false),
                    YearsFromRegistrationDateYears = table.Column<string>(type: "text", nullable: true),
                    YearsFromRegistrationDateYearsBS = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GatewayTransactionId = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PaymentGateway = table.Column<string>(type: "text", nullable: false),
                    PaymentGatewayStatus = table.Column<string>(type: "text", nullable: true),
                    PolicyPurchaseId = table.Column<string>(type: "text", nullable: true),
                    RawResponse = table.Column<string>(type: "text", nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PremiumCalculationConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CalculationEngineType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    FiscalYear = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PortfolioAlias = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremiumCalculationConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrivateVehicles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AccessoriesDetail = table.Column<string>(type: "text", nullable: true),
                    AccidentOrLossInThreeYears = table.Column<string>(type: "text", nullable: true),
                    AgeForPrint = table.Column<string>(type: "text", nullable: true),
                    AgeForPrintEnglish = table.Column<string>(type: "text", nullable: true),
                    AgeOfVehicle = table.Column<decimal>(type: "numeric", nullable: true),
                    AnyDisabilityOfEyeOrEarOfDriverCrimeAccustaion = table.Column<string>(type: "text", nullable: true),
                    AnyOtherInsuranceProposedVehicle = table.Column<string>(type: "text", nullable: true),
                    ChasisNumber = table.Column<string>(type: "text", nullable: true),
                    CompulsoryExcess = table.Column<decimal>(type: "numeric", nullable: true),
                    CopyOfPolicyIfOtherVehicleAreInsuredInThisCompany = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CubicCapacity = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrentMarketPrice = table.Column<string>(type: "text", nullable: true),
                    DateOfPurchase = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Days = table.Column<string>(type: "text", nullable: true),
                    Depreciation = table.Column<bool>(type: "boolean", nullable: false),
                    DepreciationAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    DepreciationRate = table.Column<decimal>(type: "numeric", nullable: false),
                    EngineNumber = table.Column<string>(type: "text", nullable: true),
                    EnterSumInsured = table.Column<bool>(type: "boolean", nullable: false),
                    EntitledForNoClaimDiscountNCDFromOtherInsuranceCompany = table.Column<string>(type: "text", nullable: true),
                    GoodsCarryingCapacity = table.Column<decimal>(type: "numeric", nullable: false),
                    HasAnyComputerOrInsurer = table.Column<string>(type: "text", nullable: true),
                    HasProposersOrAnyOtherPersonsDrivingLicenseEverBeenCancelled = table.Column<string>(type: "text", nullable: true),
                    HasSmartPolicy = table.Column<bool>(type: "boolean", nullable: false),
                    HasTailor = table.Column<bool>(type: "boolean", nullable: false),
                    ISRecoveryCharge = table.Column<bool>(type: "boolean", nullable: false),
                    InsuranceCompanyName = table.Column<string>(type: "text", nullable: true),
                    IsAgentInvolved = table.Column<bool>(type: "boolean", nullable: false),
                    IsComprehensive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsDifferentlyAble = table.Column<bool>(type: "boolean", nullable: false),
                    IsDirectDiscountPA = table.Column<bool>(type: "boolean", nullable: false),
                    IsEntitledForNoClaimDiscountFromOtherInsuranceCompany = table.Column<bool>(type: "boolean", nullable: false),
                    IsIssued = table.Column<bool>(type: "boolean", nullable: false),
                    IsLayup = table.Column<bool>(type: "boolean", nullable: false),
                    IsPersonalAccidentForPaidForDriver = table.Column<bool>(type: "boolean", nullable: false),
                    IsPersonalAccidentForPaidForPassenger = table.Column<bool>(type: "boolean", nullable: false),
                    IsPrivateTaxi = table.Column<bool>(type: "boolean", nullable: false),
                    IsProRataOrShortScale = table.Column<bool>(type: "boolean", nullable: false),
                    IsRiotStrikeAndTerrorismForDriver = table.Column<bool>(type: "boolean", nullable: false),
                    IsRiotStrikeAndTerrorismForPassenger = table.Column<bool>(type: "boolean", nullable: false),
                    IsThirdParty = table.Column<bool>(type: "boolean", nullable: false),
                    IsVehicleDutyFree = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LayupDays = table.Column<int>(type: "integer", nullable: false),
                    LayupDaysEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LayupDaysStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Maintenance = table.Column<string>(type: "text", nullable: true),
                    ManufactureCompany = table.Column<string>(type: "text", nullable: true),
                    ManufactureYear = table.Column<string>(type: "text", nullable: true),
                    MasterPolicyNumber = table.Column<string>(type: "text", nullable: true),
                    Model = table.Column<string>(type: "text", nullable: true),
                    NCDYears = table.Column<int>(type: "integer", nullable: false),
                    NumberofSeatsIncludingDriver = table.Column<int>(type: "integer", nullable: false),
                    PaToRiderAndOnePillionRiderSumInsuredAmount = table.Column<string>(type: "text", nullable: true),
                    ParkingGarageOpen = table.Column<string>(type: "text", nullable: true),
                    ParkingPlaceGarage = table.Column<string>(type: "text", nullable: true),
                    PartyId = table.Column<string>(type: "text", nullable: true),
                    PersonalAccidentForPassengerSeatCount = table.Column<int>(type: "integer", nullable: false),
                    PortfolioId = table.Column<string>(type: "text", nullable: true),
                    PreviousPolicyIssuedYear = table.Column<int>(type: "integer", nullable: false),
                    ProposerName = table.Column<string>(type: "text", nullable: true),
                    PurchasedNewOld = table.Column<bool>(type: "boolean", nullable: false),
                    PurposedVehicleUsedOtherThanThePurposer = table.Column<string>(type: "text", nullable: true),
                    RateOfDepreciation = table.Column<decimal>(type: "numeric", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: true),
                    RegistrationNumberNepali = table.Column<string>(type: "text", nullable: true),
                    RenewalNoticeNCD = table.Column<string>(type: "text", nullable: true),
                    Replacement = table.Column<bool>(type: "boolean", nullable: false),
                    ReplacementAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ReplacementRate = table.Column<decimal>(type: "numeric", nullable: false),
                    RiotStrikeAndTerrorism = table.Column<bool>(type: "boolean", nullable: false),
                    RiotStrikeAndTerrorismForPassengerSeatCount = table.Column<int>(type: "integer", nullable: false),
                    RiskType = table.Column<string>(type: "text", nullable: true),
                    SpecialDiscountRate = table.Column<decimal>(type: "numeric", nullable: true),
                    SubModel = table.Column<string>(type: "text", nullable: true),
                    SumInsuredAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    SumInsuredAmountForPaidDriver = table.Column<string>(type: "text", nullable: true),
                    SumInsuredAmountForPassenger = table.Column<string>(type: "text", nullable: true),
                    TotalExcess = table.Column<decimal>(type: "numeric", nullable: true),
                    Transportation = table.Column<bool>(type: "boolean", nullable: false),
                    TransportationAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TransportationRate = table.Column<decimal>(type: "numeric", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    TypeOfInsurance = table.Column<int>(type: "integer", nullable: false),
                    UseOfPrivateHire = table.Column<bool>(type: "boolean", nullable: false),
                    ValueOfAccessories = table.Column<string>(type: "text", nullable: true),
                    ValueOfTailor = table.Column<int>(type: "integer", nullable: true),
                    ValueWithoutAccessories = table.Column<string>(type: "text", nullable: true),
                    VehicleForHireOrReward = table.Column<string>(type: "text", nullable: true),
                    VehiclePurpose = table.Column<string>(type: "text", nullable: true),
                    VoluntaryExcess = table.Column<decimal>(type: "numeric", nullable: false),
                    YearsFromRegistrationDateYears = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    YearsFromRegistrationDateYearsBS = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateVehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyRiskConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PropertyDescription = table.Column<string>(type: "text", nullable: true),
                    Rate = table.Column<decimal>(type: "numeric", nullable: false),
                    RateCode = table.Column<string>(type: "text", nullable: true),
                    RiskCode = table.Column<string>(type: "text", nullable: true),
                    RiskType = table.Column<string>(type: "text", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyRiskConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertySubsidySILimits",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    SILabel = table.Column<string>(type: "text", nullable: true),
                    SILimit = table.Column<decimal>(type: "numeric", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertySubsidySILimits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RenewalReminders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Channel = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PolicyId = table.Column<string>(type: "text", nullable: true),
                    ReminderSentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RenewalReminders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ITIFamilyMembers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    InternationalTravelInsuranceId = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PassportNumber = table.Column<string>(type: "text", nullable: true),
                    Relation = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITIFamilyMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ITIFamilyMembers_ITI_InternationalTravelInsuranceId",
                        column: x => x.InternationalTravelInsuranceId,
                        principalTable: "ITI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quotations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LeadId = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "numeric", nullable: true),
                    InsuranceType = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsDirectBusiness = table.Column<bool>(type: "boolean", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Number = table.Column<string>(type: "text", nullable: true),
                    PdfUrl = table.Column<string>(type: "text", nullable: true),
                    PortfolioAlias = table.Column<string>(type: "text", nullable: true),
                    PremiumCalculationJson = table.Column<string>(type: "text", nullable: true),
                    ProductId = table.Column<string>(type: "text", nullable: true),
                    ProspectId = table.Column<string>(type: "text", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    SnapshotJson = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    SumInsured = table.Column<decimal>(type: "numeric", nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    TotalPremium = table.Column<decimal>(type: "numeric", nullable: true),
                    ValidUntil = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotations_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PremiumCalculationParameters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ConfigurationId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DefaultValue = table.Column<string>(type: "text", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxValue = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    MinValue = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ParameterKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ParameterName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    TenantId = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: false)
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
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataJson = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LookupKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    SchemaJson = table.Column<string>(type: "text", nullable: false),
                    TableName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true)
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
                    Condition = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Expression = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    RuleName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RuleType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "PolicyDrafts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CustomerId = table.Column<string>(type: "text", nullable: true),
                    ITIId = table.Column<string>(type: "text", nullable: true),
                    MotorId = table.Column<string>(type: "text", nullable: true),
                    PaymentTransactionId = table.Column<string>(type: "text", nullable: true),
                    PrivateVehicleId = table.Column<string>(type: "text", nullable: true),
                    BancassuanceBankBranch = table.Column<string>(type: "text", nullable: true),
                    BancassuanceBankName = table.Column<string>(type: "text", nullable: true),
                    BasicPremium = table.Column<decimal>(type: "numeric", nullable: false),
                    BranchCode = table.Column<string>(type: "text", nullable: true),
                    Class = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DocumentNumber = table.Column<string>(type: "text", nullable: true),
                    DraftNo = table.Column<string>(type: "text", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GovernmentSubsidyAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    GrossPremium = table.Column<decimal>(type: "numeric", nullable: false),
                    InsuranceType = table.Column<string>(type: "text", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsSubmitted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NetPremium = table.Column<decimal>(type: "numeric", nullable: false),
                    PayableAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentGateway = table.Column<string>(type: "text", nullable: true),
                    PersonalAccidentPremium = table.Column<decimal>(type: "numeric", nullable: false),
                    PolicyNumber = table.Column<string>(type: "text", nullable: true),
                    PortfolioAlias = table.Column<string>(type: "text", nullable: true),
                    PortfolioId = table.Column<string>(type: "text", nullable: true),
                    PortfolioParent = table.Column<string>(type: "text", nullable: true),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RSMDTPremium = table.Column<decimal>(type: "numeric", nullable: false),
                    ReceiptNumber = table.Column<string>(type: "text", nullable: true),
                    StampDuty = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SumInsured = table.Column<decimal>(type: "numeric", nullable: false),
                    ThirdPartyPremium = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalPremium = table.Column<decimal>(type: "numeric", nullable: false),
                    TransactionReference = table.Column<string>(type: "text", nullable: true),
                    TypeOfParty = table.Column<string>(type: "text", nullable: true),
                    VatAmount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyDrafts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolicyDrafts_ITI_ITIId",
                        column: x => x.ITIId,
                        principalTable: "ITI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolicyDrafts_Motors_MotorId",
                        column: x => x.MotorId,
                        principalTable: "Motors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolicyDrafts_PaymentTransactions_PaymentTransactionId",
                        column: x => x.PaymentTransactionId,
                        principalTable: "PaymentTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolicyDrafts_PrivateVehicles_PrivateVehicleId",
                        column: x => x.PrivateVehicleId,
                        principalTable: "PrivateVehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuotationItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CoverageId = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Premium = table.Column<decimal>(type: "numeric", nullable: false),
                    QuotationId = table.Column<string>(type: "text", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    SumInsured = table.Column<decimal>(type: "numeric", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotationItems_Quotations_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Corporates_UserId",
                table: "Corporates",
                column: "UserId");

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
                name: "IX_Fodos_FullName",
                table: "Fodos",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Fodos_UserId",
                table: "Fodos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ITIFamilyMembers_InternationalTravelInsuranceId",
                table: "ITIFamilyMembers",
                column: "InternationalTravelInsuranceId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_ProspectId",
                table: "Leads",
                column: "ProspectId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyDrafts_CustomerId",
                table: "PolicyDrafts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyDrafts_ITIId",
                table: "PolicyDrafts",
                column: "ITIId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyDrafts_MotorId",
                table: "PolicyDrafts",
                column: "MotorId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyDrafts_PaymentTransactionId",
                table: "PolicyDrafts",
                column: "PaymentTransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyDrafts_PrivateVehicleId",
                table: "PolicyDrafts",
                column: "PrivateVehicleId");

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

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItems_QuotationId",
                table: "QuotationItems",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_LeadId",
                table: "Quotations",
                column: "LeadId");
        }
    }
}
