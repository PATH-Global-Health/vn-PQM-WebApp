using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PQM_WebApp.Data.Migrations
{
    public partial class PQM_DB_v3_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataPermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    ProvinceId = table.Column<Guid>(nullable: false),
                    IndicatorId = table.Column<Guid>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataPermissions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataPermissions");
        }
    }
}
