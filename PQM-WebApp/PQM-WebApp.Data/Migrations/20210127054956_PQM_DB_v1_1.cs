using Microsoft.EntityFrameworkCore.Migrations;

namespace PQM_WebApp.Data.Migrations
{
    public partial class PQM_DB_v1_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "To",
                table: "AgeGroups",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<byte>(
                name: "From",
                table: "AgeGroups",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "To",
                table: "AgeGroups",
                type: "real",
                nullable: false,
                oldClrType: typeof(byte));

            migrationBuilder.AlterColumn<float>(
                name: "From",
                table: "AgeGroups",
                type: "real",
                nullable: false,
                oldClrType: typeof(byte));
        }
    }
}
