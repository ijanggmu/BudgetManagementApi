using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Core
{
    /// <inheritdoc />
    public partial class addedtwofasetupstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TwoFaSetupStatus",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "NotStarted");

            migrationBuilder.Sql("UPDATE \"Users\" SET \"TwoFaSetupStatus\" = 'NotStarted' WHERE \"TwoFaSetupStatus\" IS NULL;");
            migrationBuilder.Sql("UPDATE \"Users\" SET \"TwoFactorEnabled\" = FALSE ;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFaSetupStatus",
                table: "Users");
        }
    }
}
