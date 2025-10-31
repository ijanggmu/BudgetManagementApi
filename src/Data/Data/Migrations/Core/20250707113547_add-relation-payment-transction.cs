using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class addrelationpaymenttransction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_PolicyDrafts_PolicyDraftId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_PolicyDraftId",
                table: "PaymentTransactions");

            migrationBuilder.DropColumn(
                name: "PolicyDraftId",
                table: "PaymentTransactions");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "PolicyDrafts",
                newName: "PaymentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PolicyDrafts_PaymentTransactionId",
                table: "PolicyDrafts",
                column: "PaymentTransactionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PolicyDrafts_PaymentTransactions_PaymentTransactionId",
                table: "PolicyDrafts",
                column: "PaymentTransactionId",
                principalTable: "PaymentTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PolicyDrafts_PaymentTransactions_PaymentTransactionId",
                table: "PolicyDrafts");

            migrationBuilder.DropIndex(
                name: "IX_PolicyDrafts_PaymentTransactionId",
                table: "PolicyDrafts");

            migrationBuilder.RenameColumn(
                name: "PaymentTransactionId",
                table: "PolicyDrafts",
                newName: "TransactionId");

            migrationBuilder.AddColumn<string>(
                name: "PolicyDraftId",
                table: "PaymentTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PolicyDraftId",
                table: "PaymentTransactions",
                column: "PolicyDraftId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_PolicyDrafts_PolicyDraftId",
                table: "PaymentTransactions",
                column: "PolicyDraftId",
                principalTable: "PolicyDrafts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
