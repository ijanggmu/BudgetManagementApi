using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class addedkyccustomersyncstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaritialStatus",
                table: "Customers",
                newName: "MaritalStatus");

            migrationBuilder.AddColumn<string>(
                name: "ITIId",
                table: "PolicyDrafts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSubmitted",
                table: "PolicyDrafts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "privateVehicleId",
                table: "PolicyDrafts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentGatewayStatus",
                table: "PaymentTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoreSyncStatus",
                table: "Customers",
                type: "text",
                nullable: false,
                defaultValue: "NotSynced");

            migrationBuilder.AddColumn<string>(
                name: "KycStatus",
                table: "Customers",
                type: "text",
                nullable: false,
                defaultValue: "NotSubmitted");

            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    From = table.Column<string>(type: "text", nullable: true),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    To = table.Column<string>(type: "text", nullable: true),
                    Cc = table.Column<string>(type: "text", nullable: true),
                    Bcc = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "text", nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    SentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    EmailType = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ITI",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PolicyPeriodInDays = table.Column<int>(type: "integer", nullable: false),
                    TripType = table.Column<string>(type: "text", nullable: true),
                    PassportNumber = table.Column<string>(type: "text", nullable: true),
                    VisitingCountry = table.Column<string>(type: "text", nullable: true),
                    FatherHusbandName = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    Occupation = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    EmergencyContactName = table.Column<string>(type: "text", nullable: true),
                    EmergencyContactNumber = table.Column<string>(type: "text", nullable: true),
                    TravellingCountry = table.Column<string>(type: "text", nullable: true),
                    InsuranceType = table.Column<string>(type: "text", nullable: true),
                    TypeOfInsured = table.Column<string>(type: "text", nullable: true),
                    Province = table.Column<string>(type: "text", nullable: true),
                    District = table.Column<string>(type: "text", nullable: true),
                    Municipality = table.Column<string>(type: "text", nullable: true),
                    Ward = table.Column<string>(type: "text", nullable: true),
                    StreetAddress = table.Column<string>(type: "text", nullable: true),
                    PremiumAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITI", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrivateVehicles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IsComprehensive = table.Column<bool>(type: "boolean", nullable: false),
                    IsThirdParty = table.Column<bool>(type: "boolean", nullable: false),
                    EnterSumInsured = table.Column<bool>(type: "boolean", nullable: false),
                    CompulsoryExcess = table.Column<decimal>(type: "numeric", nullable: true),
                    TotalExcess = table.Column<decimal>(type: "numeric", nullable: true),
                    PartyId = table.Column<string>(type: "text", nullable: true),
                    PortfolioId = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    IsLayup = table.Column<bool>(type: "boolean", nullable: false),
                    IsDirectDiscountPA = table.Column<bool>(type: "boolean", nullable: false),
                    LayupDaysStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LayupDaysEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LayupDays = table.Column<int>(type: "integer", nullable: false),
                    ManufactureYear = table.Column<string>(type: "text", nullable: true),
                    ManufactureCompany = table.Column<string>(type: "text", nullable: true),
                    Model = table.Column<string>(type: "text", nullable: true),
                    SubModel = table.Column<string>(type: "text", nullable: true),
                    PurchasedNewOld = table.Column<bool>(type: "boolean", nullable: false),
                    DateOfPurchase = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ChasisNumber = table.Column<string>(type: "text", nullable: true),
                    EngineNumber = table.Column<string>(type: "text", nullable: true),
                    AgeForPrint = table.Column<string>(type: "text", nullable: true),
                    AgeForPrintEnglish = table.Column<string>(type: "text", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: true),
                    RegistrationNumberNepali = table.Column<string>(type: "text", nullable: true),
                    ProposerName = table.Column<string>(type: "text", nullable: true),
                    TypeOfInsurance = table.Column<int>(type: "integer", nullable: false),
                    GoodsCarryingCapacity = table.Column<decimal>(type: "numeric", nullable: false),
                    CubicCapacity = table.Column<decimal>(type: "numeric", nullable: false),
                    VoluntaryExcess = table.Column<decimal>(type: "numeric", nullable: false),
                    RiotStrikeAndTerrorism = table.Column<bool>(type: "boolean", nullable: false),
                    YearsFromRegistrationDateYears = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    YearsFromRegistrationDateYearsBS = table.Column<string>(type: "text", nullable: true),
                    CurrentMarketPrice = table.Column<string>(type: "text", nullable: true),
                    IsPersonalAccidentForPaidForDriver = table.Column<bool>(type: "boolean", nullable: false),
                    IsPersonalAccidentForPaidForPassenger = table.Column<bool>(type: "boolean", nullable: false),
                    PersonalAccidentForPassengerSeatCount = table.Column<int>(type: "integer", nullable: false),
                    NumberofSeatsIncludingDriver = table.Column<int>(type: "integer", nullable: false),
                    SumInsuredAmountForPaidDriver = table.Column<string>(type: "text", nullable: true),
                    SumInsuredAmountForPassenger = table.Column<string>(type: "text", nullable: true),
                    HasTailor = table.Column<bool>(type: "boolean", nullable: false),
                    ValueOfTailor = table.Column<int>(type: "integer", nullable: true),
                    UseOfPrivateHire = table.Column<bool>(type: "boolean", nullable: false),
                    AgeOfVehicle = table.Column<decimal>(type: "numeric", nullable: true),
                    RateOfDepreciation = table.Column<decimal>(type: "numeric", nullable: true),
                    ValueOfAccessories = table.Column<string>(type: "text", nullable: true),
                    ValueWithoutAccessories = table.Column<string>(type: "text", nullable: true),
                    SumInsuredAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsVehicleDutyFree = table.Column<bool>(type: "boolean", nullable: false),
                    VehiclePurpose = table.Column<string>(type: "text", nullable: true),
                    AccessoriesDetail = table.Column<string>(type: "text", nullable: true),
                    IsProRataOrShortScale = table.Column<bool>(type: "boolean", nullable: false),
                    Days = table.Column<string>(type: "text", nullable: true),
                    NCDYears = table.Column<int>(type: "integer", nullable: false),
                    VehicleForHireOrReward = table.Column<string>(type: "text", nullable: true),
                    ParkingPlaceGarage = table.Column<string>(type: "text", nullable: true),
                    ParkingGarageOpen = table.Column<string>(type: "text", nullable: true),
                    Maintenance = table.Column<string>(type: "text", nullable: true),
                    PurposedVehicleUsedOtherThanThePurposer = table.Column<string>(type: "text", nullable: true),
                    AnyDisabilityOfEyeOrEarOfDriverCrimeAccustaion = table.Column<string>(type: "text", nullable: true),
                    AnyOtherInsuranceProposedVehicle = table.Column<string>(type: "text", nullable: true),
                    InsuranceCompanyName = table.Column<string>(type: "text", nullable: true),
                    IsEntitledForNoClaimDiscountFromOtherInsuranceCompany = table.Column<bool>(type: "boolean", nullable: false),
                    EntitledForNoClaimDiscountNCDFromOtherInsuranceCompany = table.Column<string>(type: "text", nullable: true),
                    RenewalNoticeNCD = table.Column<string>(type: "text", nullable: true),
                    HasAnyComputerOrInsurer = table.Column<string>(type: "text", nullable: true),
                    AccidentOrLossInThreeYears = table.Column<string>(type: "text", nullable: true),
                    HasProposersOrAnyOtherPersonsDrivingLicenseEverBeenCancelled = table.Column<string>(type: "text", nullable: true),
                    CopyOfPolicyIfOtherVehicleAreInsuredInThisCompany = table.Column<string>(type: "text", nullable: true),
                    RiskType = table.Column<string>(type: "text", nullable: true),
                    IsIssued = table.Column<bool>(type: "boolean", nullable: false),
                    IsAgentInvolved = table.Column<bool>(type: "boolean", nullable: false),
                    PreviousPolicyIssuedYear = table.Column<int>(type: "integer", nullable: false),
                    PaToRiderAndOnePillionRiderSumInsuredAmount = table.Column<string>(type: "text", nullable: true),
                    IsPrivateTaxi = table.Column<bool>(type: "boolean", nullable: false),
                    IsRiotStrikeAndTerrorismForDriver = table.Column<bool>(type: "boolean", nullable: false),
                    IsRiotStrikeAndTerrorismForPassenger = table.Column<bool>(type: "boolean", nullable: false),
                    RiotStrikeAndTerrorismForPassengerSeatCount = table.Column<int>(type: "integer", nullable: false),
                    IsDifferentlyAble = table.Column<bool>(type: "boolean", nullable: false),
                    SpecialDiscountRate = table.Column<decimal>(type: "numeric", nullable: true),
                    ISRecoveryCharge = table.Column<bool>(type: "boolean", nullable: false),
                    MasterPolicyNumber = table.Column<string>(type: "text", nullable: true),
                    Transportation = table.Column<bool>(type: "boolean", nullable: false),
                    Replacement = table.Column<bool>(type: "boolean", nullable: false),
                    Depreciation = table.Column<bool>(type: "boolean", nullable: false),
                    TransportationRate = table.Column<decimal>(type: "numeric", nullable: false),
                    TransportationAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ReplacementRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ReplacementAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    DepreciationRate = table.Column<decimal>(type: "numeric", nullable: false),
                    DepreciationAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    HasSmartPolicy = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateVehicles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmsLogs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    From = table.Column<string>(type: "text", nullable: true),
                    To = table.Column<string>(type: "text", nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    SentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    SmsType = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsLogs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PolicyDrafts_ITIId",
                table: "PolicyDrafts",
                column: "ITIId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyDrafts_privateVehicleId",
                table: "PolicyDrafts",
                column: "privateVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PanNo",
                table: "Customers",
                column: "PanNo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PolicyDrafts_ITI_ITIId",
                table: "PolicyDrafts",
                column: "ITIId",
                principalTable: "ITI",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PolicyDrafts_PrivateVehicles_privateVehicleId",
                table: "PolicyDrafts",
                column: "privateVehicleId",
                principalTable: "PrivateVehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("UPDATE \"Customers\" SET \"KycStatus\" = 'NotSubmitted' WHERE \"KycStatus\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"Customers\" SET \"CoreSyncStatus\" = 'NotSynced' WHERE \"CoreSyncStatus\" IS NULL;");
            migrationBuilder.Sql("CREATE SEQUENCE IF NOT EXISTS draft_number_seq START 1 INCREMENT 1;");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP SEQUENCE IF EXISTS draft_number_seq;");

            migrationBuilder.DropForeignKey(
                name: "FK_PolicyDrafts_ITI_ITIId",
                table: "PolicyDrafts");

            migrationBuilder.DropForeignKey(
                name: "FK_PolicyDrafts_PrivateVehicles_privateVehicleId",
                table: "PolicyDrafts");

            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "ITI");

            migrationBuilder.DropTable(
                name: "PrivateVehicles");

            migrationBuilder.DropTable(
                name: "SmsLogs");

            migrationBuilder.DropIndex(
                name: "IX_PolicyDrafts_ITIId",
                table: "PolicyDrafts");

            migrationBuilder.DropIndex(
                name: "IX_PolicyDrafts_privateVehicleId",
                table: "PolicyDrafts");

            migrationBuilder.DropIndex(
                name: "IX_Customers_PanNo",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ITIId",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "IsSubmitted",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "privateVehicleId",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "PaymentGatewayStatus",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "CoreSyncStatus",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "KycStatus",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "MaritalStatus",
                table: "Customers",
                newName: "MaritialStatus");
        }
    }
}
