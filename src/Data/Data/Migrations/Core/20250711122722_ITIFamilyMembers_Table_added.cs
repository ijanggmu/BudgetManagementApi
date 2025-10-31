using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class ITIFamilyMembers_Table_added : Migration
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

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveDateBeforeEndorsement",
                table: "ITI",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDateBeforeEndorsement",
                table: "ITI",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "IdType",
                table: "ITI",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ITIFamilyMembers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Relation = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    PassportNumber = table.Column<string>(type: "text", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: true),
                    InternationalTravelInsuranceId = table.Column<int>(type: "integer", nullable: false),
                    InternationalTravelInsuranceId1 = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITIFamilyMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ITIFamilyMembers_ITI_InternationalTravelInsuranceId1",
                        column: x => x.InternationalTravelInsuranceId1,
                        principalTable: "ITI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ITIFamilyMembers_InternationalTravelInsuranceId1",
                table: "ITIFamilyMembers",
                column: "InternationalTravelInsuranceId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "YearsFromRegistrationDateYears",
                table: "Motors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.DropTable(
                name: "ITIFamilyMembers");

            migrationBuilder.DropColumn(
                name: "EffectiveDateBeforeEndorsement",
                table: "ITI");

            migrationBuilder.DropColumn(
                name: "ExpiryDateBeforeEndorsement",
                table: "ITI");

            migrationBuilder.DropColumn(
                name: "IdType",
                table: "ITI");
        }
    }
}
