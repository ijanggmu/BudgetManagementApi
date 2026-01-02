using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class removerolenameuniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyBrandings_Tenants_TenantId1",
                table: "CompanyBrandings");

            migrationBuilder.DropForeignKey(
                name: "FK_ITIFamilyMembers_ITI_InternationalTravelInsuranceId1",
                table: "ITIFamilyMembers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_ITIFamilyMembers_InternationalTravelInsuranceId1",
                table: "ITIFamilyMembers");

            migrationBuilder.DropIndex(
                name: "IX_CompanyBrandings_TenantId1",
                table: "CompanyBrandings");

            migrationBuilder.DropColumn(
                name: "InternationalTravelInsuranceId1",
                table: "ITIFamilyMembers");

            migrationBuilder.DropColumn(
                name: "TenantId1",
                table: "CompanyBrandings");

            migrationBuilder.AlterColumn<string>(
                name: "InternationalTravelInsuranceId",
                table: "ITIFamilyMembers",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_NormalizedName_TenantId",
                table: "Roles",
                columns: new[] { "NormalizedName", "TenantId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_ITIFamilyMembers_InternationalTravelInsuranceId",
                table: "ITIFamilyMembers",
                column: "InternationalTravelInsuranceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ITIFamilyMembers_ITI_InternationalTravelInsuranceId",
                table: "ITIFamilyMembers",
                column: "InternationalTravelInsuranceId",
                principalTable: "ITI",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ITIFamilyMembers_ITI_InternationalTravelInsuranceId",
                table: "ITIFamilyMembers");

            migrationBuilder.DropIndex(
                name: "IX_Roles_NormalizedName_TenantId",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_ITIFamilyMembers_InternationalTravelInsuranceId",
                table: "ITIFamilyMembers");

            migrationBuilder.AlterColumn<int>(
                name: "InternationalTravelInsuranceId",
                table: "ITIFamilyMembers",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternationalTravelInsuranceId1",
                table: "ITIFamilyMembers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TenantId1",
                table: "CompanyBrandings",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ITIFamilyMembers_InternationalTravelInsuranceId1",
                table: "ITIFamilyMembers",
                column: "InternationalTravelInsuranceId1");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyBrandings_TenantId1",
                table: "CompanyBrandings",
                column: "TenantId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyBrandings_Tenants_TenantId1",
                table: "CompanyBrandings",
                column: "TenantId1",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ITIFamilyMembers_ITI_InternationalTravelInsuranceId1",
                table: "ITIFamilyMembers",
                column: "InternationalTravelInsuranceId1",
                principalTable: "ITI",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
