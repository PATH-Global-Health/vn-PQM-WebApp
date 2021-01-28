using Microsoft.EntityFrameworkCore.Migrations;

namespace PQM_WebApp.Data.Migrations
{
    public partial class PQM_DB_v1_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameWithType",
                table: "Provinces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Provinces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Provinces",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameWithType",
                table: "Districts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentCode",
                table: "Districts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Districts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PathWithType",
                table: "Districts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Districts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Districts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameWithType",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "NameWithType",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "ParentCode",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "PathWithType",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Districts");
        }
    }
}
