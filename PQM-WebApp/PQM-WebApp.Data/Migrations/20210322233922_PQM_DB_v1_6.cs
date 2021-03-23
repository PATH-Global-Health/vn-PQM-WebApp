using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PQM_WebApp.Data.Migrations
{
    public partial class PQM_DB_v1_6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "Districts",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Lng",
                table: "Districts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ThresholdSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    DistrictCode = table.Column<string>(nullable: true),
                    ProvinceCode = table.Column<string>(nullable: true),
                    IndicatorNamge = table.Column<string>(nullable: true),
                    From = table.Column<double>(nullable: false),
                    To = table.Column<double>(nullable: false),
                    ColorCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThresholdSettings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThresholdSettings");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "Districts");
        }
    }
}
