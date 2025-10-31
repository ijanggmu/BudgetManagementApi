using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class addressTablemodified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PolicyDrafts_Addresses_AddressID",
                table: "PolicyDrafts");

            migrationBuilder.DropIndex(
                name: "IX_PolicyDrafts_AddressID",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "AddressID",
                table: "PolicyDrafts");

            migrationBuilder.AddColumn<string>(
                name: "PolicyDraftId",
                table: "Addresses",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_PolicyDraftId",
                table: "Addresses",
                column: "PolicyDraftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_PolicyDrafts_PolicyDraftId",
                table: "Addresses",
                column: "PolicyDraftId",
                principalTable: "PolicyDrafts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_PolicyDrafts_PolicyDraftId",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_PolicyDraftId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "PolicyDraftId",
                table: "Addresses");

            migrationBuilder.AddColumn<string>(
                name: "AddressID",
                table: "PolicyDrafts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PolicyDrafts_AddressID",
                table: "PolicyDrafts",
                column: "AddressID");

            migrationBuilder.AddForeignKey(
                name: "FK_PolicyDrafts_Addresses_AddressID",
                table: "PolicyDrafts",
                column: "AddressID",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
