using Microsoft.EntityFrameworkCore.Migrations;

namespace PQM_WebApp.Data.Migrations
{
    public partial class PQM_v1_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Value",
                table: "AggregatedValues",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Denominator",
                table: "AggregatedValues",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Numerator",
                table: "AggregatedValues",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Denominator",
                table: "AggregatedValues");

            migrationBuilder.DropColumn(
                name: "Numerator",
                table: "AggregatedValues");

            migrationBuilder.AlterColumn<int>(
                name: "Value",
                table: "AggregatedValues",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
