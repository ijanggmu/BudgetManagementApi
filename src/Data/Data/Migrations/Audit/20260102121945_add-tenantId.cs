using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Audit
{
    /// <inheritdoc />
    public partial class addtenantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "UserActivities",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_TenantId",
                table: "UserActivities",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserActivities_TenantId",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserActivities");
        }
    }
}
