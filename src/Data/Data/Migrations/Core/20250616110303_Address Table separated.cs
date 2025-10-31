using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class AddressTableseparated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CitizenshipIssueDate",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CitizenshipIssueDistrict",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CitizenshipNo",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientClassification",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourtesyTitle",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DobAD",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DobBS",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName_Np",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IndividualType",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName_Np",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaritialStatus",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName_Np",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "OccupationJson",
                table: "Customers",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PanNo",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportExpiryDate",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PassportIssueDate",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportIssuePlace",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportNumber",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VoterIdNumber",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Province = table.Column<string>(type: "text", nullable: true),
                    District = table.Column<string>(type: "text", nullable: true),
                    Municipality = table.Column<string>(type: "text", nullable: true),
                    Ward = table.Column<string>(type: "text", nullable: true),
                    StreetAddress = table.Column<string>(type: "text", nullable: true),
                    AddressType = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Motors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    IsThirdParty = table.Column<bool>(type: "boolean", nullable: false),
                    IsComprehensive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    PartyId = table.Column<string>(type: "text", nullable: true),
                    ManufactureYear = table.Column<string>(type: "text", nullable: true),
                    ManufactureCompany = table.Column<string>(type: "text", nullable: true),
                    Model = table.Column<string>(type: "text", nullable: true),
                    SubModel = table.Column<string>(type: "text", nullable: true),
                    PurchasedNewOld = table.Column<bool>(type: "boolean", nullable: false),
                    DateOfPurchase = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChasisNumber = table.Column<string>(type: "text", nullable: true),
                    EngineNumber = table.Column<string>(type: "text", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: true),
                    VoluntaryExcess = table.Column<decimal>(type: "numeric", nullable: false),
                    CompulsoryExcess = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalExcess = table.Column<decimal>(type: "numeric", nullable: false),
                    CubicCapacity = table.Column<string>(type: "text", nullable: true),
                    Days = table.Column<int>(type: "integer", nullable: false),
                    YearsFromRegistrationDateYears = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    YearsFromRegistrationDateYearsBS = table.Column<string>(type: "text", nullable: true),
                    CurrentMarketPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    AgeOfVehicle = table.Column<int>(type: "integer", nullable: true),
                    RateOfDepreciation = table.Column<decimal>(type: "numeric", nullable: true),
                    ValueOfAccessories = table.Column<decimal>(type: "numeric", nullable: true),
                    VehicleForHireOrReward = table.Column<bool>(type: "boolean", nullable: true),
                    ParkingPlaceGarage = table.Column<bool>(type: "boolean", nullable: true),
                    ParkingGarageOpen = table.Column<bool>(type: "boolean", nullable: true),
                    Maintenance = table.Column<bool>(type: "boolean", nullable: true),
                    NCDYears = table.Column<int>(type: "integer", nullable: false),
                    NumberofSeatsIncludingDriver = table.Column<int>(type: "integer", nullable: false),
                    RiotStrike = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Motors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PolicyDrafts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DraftNo = table.Column<string>(type: "text", nullable: true),
                    StaffEmail = table.Column<string>(type: "text", nullable: true),
                    GateWay = table.Column<string>(type: "text", nullable: true),
                    TransactionId = table.Column<string>(type: "text", nullable: true),
                    NetPremium = table.Column<decimal>(type: "numeric", nullable: false),
                    BranchCode = table.Column<string>(type: "text", nullable: true),
                    PortfolioAlias = table.Column<string>(type: "text", nullable: true),
                    PortfolioId = table.Column<string>(type: "text", nullable: true),
                    TypeOfParty = table.Column<string>(type: "text", nullable: true),
                    PortfolioParent = table.Column<string>(type: "text", nullable: true),
                    Class = table.Column<string>(type: "text", nullable: true),
                    User = table.Column<string>(type: "text", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProposedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPolicyFromThirdParty = table.Column<bool>(type: "boolean", nullable: false),
                    BancassuanceBankName = table.Column<string>(type: "text", nullable: true),
                    BancassuanceBankBranch = table.Column<string>(type: "text", nullable: true),
                    CustomerId = table.Column<string>(type: "text", nullable: true),
                    MotorId = table.Column<string>(type: "text", nullable: true),
                    AddressID = table.Column<string>(type: "text", nullable: true),
                    ShareCompanyLists = table.Column<List<string>>(type: "text[]", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolicyDrafts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolicyDrafts_Addresses_AddressID",
                        column: x => x.AddressID,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolicyDrafts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolicyDrafts_Motors_MotorId",
                        column: x => x.MotorId,
                        principalTable: "Motors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PolicyDrafts_AddressID",
                table: "PolicyDrafts",
                column: "AddressID");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyDrafts_CustomerId",
                table: "PolicyDrafts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyDrafts_MotorId",
                table: "PolicyDrafts",
                column: "MotorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolicyDrafts");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Motors");

            migrationBuilder.DropColumn(
                name: "CitizenshipIssueDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CitizenshipIssueDistrict",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CitizenshipNo",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ClientClassification",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CourtesyTitle",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DobAD",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DobBS",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "FirstName_Np",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IndividualType",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastName_Np",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MaritialStatus",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MiddleName_Np",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "OccupationJson",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PanNo",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PassportExpiryDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PassportIssueDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PassportIssuePlace",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PassportNumber",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "VoterIdNumber",
                table: "Customers");
        }
    }
}
