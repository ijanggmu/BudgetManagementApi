using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class removestaffemailpolicydraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GateWay",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "IsPolicyFromThirdParty",
                table: "PolicyDrafts");

            migrationBuilder.RenameColumn(
                name: "User",
                table: "PolicyDrafts",
                newName: "TransactionReference");

            migrationBuilder.RenameColumn(
                name: "StaffEmail",
                table: "PolicyDrafts",
                newName: "PaymentGateway");

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchasedAt",
                table: "PolicyDrafts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PolicyDrafts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PolicyPurchaseId = table.Column<string>(type: "text", nullable: true),
                    PolicyDraftId = table.Column<string>(type: "text", nullable: true),
                    GatewayTransactionId = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RawResponse = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_PolicyDrafts_PolicyDraftId",
                        column: x => x.PolicyDraftId,
                        principalTable: "PolicyDrafts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PolicyDraftId",
                table: "PaymentTransactions",
                column: "PolicyDraftId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "PurchasedAt",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PolicyDrafts");

            migrationBuilder.RenameColumn(
                name: "TransactionReference",
                table: "PolicyDrafts",
                newName: "User");

            migrationBuilder.RenameColumn(
                name: "PaymentGateway",
                table: "PolicyDrafts",
                newName: "StaffEmail");

            migrationBuilder.AddColumn<string>(
                name: "GateWay",
                table: "PolicyDrafts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPolicyFromThirdParty",
                table: "PolicyDrafts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
