using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PQM_WebApp.Data.Migrations
{
    public partial class PQM_DB_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgeGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    From = table.Column<byte>(nullable: true),
                    To = table.Column<byte>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategoryAliases",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CategoryId = table.Column<Guid>(nullable: false),
                    Alias = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAliases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gender",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gender", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndicatorGroups",
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
                    table.PrimaryKey("PK_IndicatorGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeyPopulations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyPopulations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    NameWithType = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Id);
                });

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
                    IndicatorName = table.Column<string>(nullable: true),
                    From = table.Column<double>(nullable: false),
                    To = table.Column<double>(nullable: false),
                    ColorCode = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThresholdSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UndefinedDimValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Dimension = table.Column<string>(nullable: true),
                    UndefinedValue = table.Column<string>(nullable: true),
                    SourceId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UndefinedDimValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Indicators",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    IsTotal = table.Column<bool>(nullable: true),
                    IndicatorGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indicators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Indicators_IndicatorGroups_IndicatorGroupId",
                        column: x => x.IndicatorGroupId,
                        principalTable: "IndicatorGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Slug = table.Column<string>(nullable: true),
                    NameWithType = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    PathWithType = table.Column<string>(nullable: true),
                    ParentCode = table.Column<string>(nullable: true),
                    Lat = table.Column<double>(nullable: true),
                    Lng = table.Column<double>(nullable: true),
                    ProvinceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_Provinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "Provinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Lat = table.Column<double>(nullable: true),
                    Lng = table.Column<double>(nullable: true),
                    DistrictId = table.Column<Guid>(nullable: false),
                    SiteTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sites_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sites_SiteTypes_SiteTypeId",
                        column: x => x.SiteTypeId,
                        principalTable: "SiteTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AggregatedValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    DataType = table.Column<int>(nullable: false),
                    Denominator = table.Column<int>(nullable: false),
                    Numerator = table.Column<int>(nullable: false),
                    PeriodType = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    Quarter = table.Column<int>(nullable: true),
                    Month = table.Column<int>(nullable: true),
                    Day = table.Column<int>(nullable: true),
                    SiteId = table.Column<Guid>(nullable: false),
                    IndicatorId = table.Column<Guid>(nullable: false),
                    GenderId = table.Column<Guid>(nullable: false),
                    AgeGroupId = table.Column<Guid>(nullable: false),
                    KeyPopulationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AggregatedValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AggregatedValues_AgeGroups_AgeGroupId",
                        column: x => x.AgeGroupId,
                        principalTable: "AgeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AggregatedValues_Gender_GenderId",
                        column: x => x.GenderId,
                        principalTable: "Gender",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AggregatedValues_Indicators_IndicatorId",
                        column: x => x.IndicatorId,
                        principalTable: "Indicators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AggregatedValues_KeyPopulations_KeyPopulationId",
                        column: x => x.KeyPopulationId,
                        principalTable: "KeyPopulations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AggregatedValues_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnsolvedDimValues",
                columns: table => new
                {
                    AggregatedValueId = table.Column<Guid>(nullable: false),
                    UndefinedDimValueId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnsolvedDimValues", x => new { x.AggregatedValueId, x.UndefinedDimValueId });
                    table.ForeignKey(
                        name: "FK_UnsolvedDimValues_AggregatedValues_AggregatedValueId",
                        column: x => x.AggregatedValueId,
                        principalTable: "AggregatedValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UnsolvedDimValues_UndefinedDimValues_UndefinedDimValueId",
                        column: x => x.UndefinedDimValueId,
                        principalTable: "UndefinedDimValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedValues_AgeGroupId",
                table: "AggregatedValues",
                column: "AgeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedValues_GenderId",
                table: "AggregatedValues",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedValues_IndicatorId",
                table: "AggregatedValues",
                column: "IndicatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedValues_KeyPopulationId",
                table: "AggregatedValues",
                column: "KeyPopulationId");

            migrationBuilder.CreateIndex(
                name: "IX_AggregatedValues_SiteId",
                table: "AggregatedValues",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_ProvinceId",
                table: "Districts",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_Indicators_IndicatorGroupId",
                table: "Indicators",
                column: "IndicatorGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_DistrictId",
                table: "Sites",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_SiteTypeId",
                table: "Sites",
                column: "SiteTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UnsolvedDimValues_UndefinedDimValueId",
                table: "UnsolvedDimValues",
                column: "UndefinedDimValueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryAliases");

            migrationBuilder.DropTable(
                name: "ThresholdSettings");

            migrationBuilder.DropTable(
                name: "UnsolvedDimValues");

            migrationBuilder.DropTable(
                name: "AggregatedValues");

            migrationBuilder.DropTable(
                name: "UndefinedDimValues");

            migrationBuilder.DropTable(
                name: "AgeGroups");

            migrationBuilder.DropTable(
                name: "Gender");

            migrationBuilder.DropTable(
                name: "Indicators");

            migrationBuilder.DropTable(
                name: "KeyPopulations");

            migrationBuilder.DropTable(
                name: "Sites");

            migrationBuilder.DropTable(
                name: "IndicatorGroups");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "SiteTypes");

            migrationBuilder.DropTable(
                name: "Provinces");
        }
    }
}
