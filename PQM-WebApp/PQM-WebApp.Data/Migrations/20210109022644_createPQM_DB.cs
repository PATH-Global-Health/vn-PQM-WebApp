using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PQM_WebApp.Data.Migrations
{
    public partial class createPQM_DB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DimYears",
                columns: table => new
                {
                    Year = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimYears", x => x.Year);
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
                    CityId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Districts_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DimQuarters",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    QuarterNumOfYear = table.Column<byte>(nullable: false),
                    BeginDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    YearId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimQuarters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DimQuarters_DimYears_YearId",
                        column: x => x.YearId,
                        principalTable: "DimYears",
                        principalColumn: "Year",
                        onDelete: ReferentialAction.Restrict);
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
                name: "Wards",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateUpdated = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    DistrictId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wards_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DimMonths",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    MonthNumOfYear = table.Column<byte>(nullable: false),
                    BeginDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    QuarterId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimMonths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DimMonths_DimQuarters_QuarterId",
                        column: x => x.QuarterId,
                        principalTable: "DimQuarters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DimWeeks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    WeekNumOfMonth = table.Column<int>(nullable: false),
                    WeekNumOfQuarter = table.Column<int>(nullable: false),
                    WeekNumOfYear = table.Column<int>(nullable: false),
                    BeginDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    MonthId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimWeeks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DimWeeks_DimMonths_MonthId",
                        column: x => x.MonthId,
                        principalTable: "DimMonths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DimDates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayOfWeekName = table.Column<string>(nullable: true),
                    DayNumOfWeek = table.Column<int>(nullable: false),
                    DayNumOfMonth = table.Column<int>(nullable: false),
                    DayNumOfQuarter = table.Column<int>(nullable: false),
                    DayNumOfYear = table.Column<int>(nullable: false),
                    WeekId = table.Column<Guid>(nullable: false),
                    MonthId = table.Column<Guid>(nullable: false),
                    QuarterId = table.Column<Guid>(nullable: false),
                    YearId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DimDates_DimMonths_MonthId",
                        column: x => x.MonthId,
                        principalTable: "DimMonths",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DimDates_DimQuarters_QuarterId",
                        column: x => x.QuarterId,
                        principalTable: "DimQuarters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DimDates_DimWeeks_WeekId",
                        column: x => x.WeekId,
                        principalTable: "DimWeeks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DimDates_DimYears_YearId",
                        column: x => x.YearId,
                        principalTable: "DimYears",
                        principalColumn: "Year",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndicatorSummaryValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IndicatorId = table.Column<Guid>(nullable: false),
                    WardId = table.Column<Guid>(nullable: true),
                    DistrictId = table.Column<Guid>(nullable: true),
                    CityId = table.Column<Guid>(nullable: false),
                    DimDateId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    ValueType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicatorSummaryValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndicatorSummaryValues_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndicatorSummaryValues_DimDates_DimDateId",
                        column: x => x.DimDateId,
                        principalTable: "DimDates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndicatorSummaryValues_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndicatorSummaryValues_Indicators_IndicatorId",
                        column: x => x.IndicatorId,
                        principalTable: "Indicators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndicatorSummaryValues_Wards_WardId",
                        column: x => x.WardId,
                        principalTable: "Wards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DimDates_MonthId",
                table: "DimDates",
                column: "MonthId");

            migrationBuilder.CreateIndex(
                name: "IX_DimDates_QuarterId",
                table: "DimDates",
                column: "QuarterId");

            migrationBuilder.CreateIndex(
                name: "IX_DimDates_WeekId",
                table: "DimDates",
                column: "WeekId");

            migrationBuilder.CreateIndex(
                name: "IX_DimDates_YearId",
                table: "DimDates",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_DimMonths_QuarterId",
                table: "DimMonths",
                column: "QuarterId");

            migrationBuilder.CreateIndex(
                name: "IX_DimQuarters_YearId",
                table: "DimQuarters",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_DimWeeks_MonthId",
                table: "DimWeeks",
                column: "MonthId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_CityId",
                table: "Districts",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Indicators_IndicatorGroupId",
                table: "Indicators",
                column: "IndicatorGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorSummaryValues_CityId",
                table: "IndicatorSummaryValues",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorSummaryValues_DimDateId",
                table: "IndicatorSummaryValues",
                column: "DimDateId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorSummaryValues_DistrictId",
                table: "IndicatorSummaryValues",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorSummaryValues_IndicatorId",
                table: "IndicatorSummaryValues",
                column: "IndicatorId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorSummaryValues_WardId",
                table: "IndicatorSummaryValues",
                column: "WardId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_DistrictId",
                table: "Wards",
                column: "DistrictId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndicatorSummaryValues");

            migrationBuilder.DropTable(
                name: "DimDates");

            migrationBuilder.DropTable(
                name: "Indicators");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "DimWeeks");

            migrationBuilder.DropTable(
                name: "IndicatorGroups");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "DimMonths");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "DimQuarters");

            migrationBuilder.DropTable(
                name: "DimYears");
        }
    }
}
