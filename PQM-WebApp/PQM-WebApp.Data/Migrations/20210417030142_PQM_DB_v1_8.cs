using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PQM_WebApp.Data.Migrations
{
    public partial class PQM_DB_v1_8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "CategoryAliases");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "CategoryAliases",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "CategoryAliases");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CategoryAliases",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
