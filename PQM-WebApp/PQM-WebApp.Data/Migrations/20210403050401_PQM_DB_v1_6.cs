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
                table: "Sites",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Lng",
                table: "Sites",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SiteTypeId",
                table: "Sites",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Indicators",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTotal",
                table: "Indicators",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Lat",
                table: "Districts",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Lng",
                table: "Districts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SiteTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sites_SiteTypeId",
                table: "Sites",
                column: "SiteTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_SiteTypes_SiteTypeId",
                table: "Sites",
                column: "SiteTypeId",
                principalTable: "SiteTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sites_SiteTypes_SiteTypeId",
                table: "Sites");

            migrationBuilder.DropTable(
                name: "SiteTypes");

            migrationBuilder.DropIndex(
                name: "IX_Sites_SiteTypeId",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "SiteTypeId",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "IsTotal",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "Districts");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "Districts");
        }
    }
}
