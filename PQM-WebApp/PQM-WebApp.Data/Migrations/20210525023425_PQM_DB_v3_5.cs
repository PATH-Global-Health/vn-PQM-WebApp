using Microsoft.EntityFrameworkCore.Migrations;

namespace PQM_WebApp.Data.Migrations
{
    public partial class PQM_DB_v3_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvalidMessage",
                table: "AggregatedValues",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "AggregatedValues",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvalidMessage",
                table: "AggregatedValues");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "AggregatedValues");
        }
    }
}
