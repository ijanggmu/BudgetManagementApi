using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class removeunusedcolumnpolicydraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PolicyDrafts_PrivateVehicles_privateVehicleId",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "ProposedDate",
                table: "PolicyDrafts");

            migrationBuilder.DropColumn(
                name: "ShareCompanyLists",
                table: "PolicyDrafts");

            migrationBuilder.RenameColumn(
                name: "privateVehicleId",
                table: "PolicyDrafts",
                newName: "PrivateVehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_PolicyDrafts_privateVehicleId",
                table: "PolicyDrafts",
                newName: "IX_PolicyDrafts_PrivateVehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_PolicyDrafts_PrivateVehicles_PrivateVehicleId",
                table: "PolicyDrafts",
                column: "PrivateVehicleId",
                principalTable: "PrivateVehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PolicyDrafts_PrivateVehicles_PrivateVehicleId",
                table: "PolicyDrafts");

            migrationBuilder.RenameColumn(
                name: "PrivateVehicleId",
                table: "PolicyDrafts",
                newName: "privateVehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_PolicyDrafts_PrivateVehicleId",
                table: "PolicyDrafts",
                newName: "IX_PolicyDrafts_privateVehicleId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ProposedDate",
                table: "PolicyDrafts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<List<string>>(
                name: "ShareCompanyLists",
                table: "PolicyDrafts",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PolicyDrafts_PrivateVehicles_privateVehicleId",
                table: "PolicyDrafts",
                column: "privateVehicleId",
                principalTable: "PrivateVehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
