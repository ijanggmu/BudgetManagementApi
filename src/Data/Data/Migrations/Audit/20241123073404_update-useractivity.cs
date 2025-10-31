using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Audit
{
    /// <inheritdoc />
    public partial class updateuseractivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrelationId",
                table: "UserActivities",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ResponseTime",
                table: "UserActivities",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "ResponseTime",
                table: "UserActivities");
        }
    }
}
