using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class CustomerIdaddedtiaddressTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
