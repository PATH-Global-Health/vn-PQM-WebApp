using System;
using PQM_WebApp.Data;
using PQM_WebApp.Data.Models;
using NPOI;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace PQM_WebApp.Service
{
    public interface IExcelService
    {
        ResultModel GetAggregatedData(int? pageIndex = 0, int? pageSize = int.MaxValue
                      , string period = null, int? year = null, int? quarter = null, int? month = null
                      , Guid? indicatorId = null, Guid? ageGroupId = null, Guid? genderId = null, Guid? keyPopulationId = null
                      , Guid? provinceId = null, Guid? districId = null, Guid? siteId = null, Guid? indicatorGroupId = null);
    }

    public class ExcelService : IExcelService
    {
        private readonly AppDBContext _dbContext;

        public ExcelService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel GetAggregatedData(int? pageIndex = 0, int? pageSize = int.MaxValue, string period = null, int? year = null, int? quarter = null, int? month = null, Guid? indicatorId = null, Guid? ageGroupId = null, Guid? genderId = null, Guid? keyPopulationId = null, Guid? provinceId = null, Guid? districId = null, Guid? siteId = null, Guid? indicatorGroupId = null)
        {
            var result = new ResultModel();
            try
            {
                var filter = _dbContext.AggregatedValues.Where(w => !w.IsDeleted)
                                                .Include(w => w.AgeGroup)
                                                .Include(w => w.Indicator)
                                                .Include(w => w.Gender)
                                                .Include(w => w.Site)
                                                .Include(w => w.KeyPopulation)
                                                .AsNoTracking();
                if (!string.IsNullOrEmpty(period))
                {
                    filter = filter.Where(w => w.PeriodType == period);
                }
                if (year != null)
                {
                    filter = filter.Where(w => w.Year == year);
                }
                if (quarter != null)
                {
                    filter = filter.Where(w => w.Quarter == quarter);
                }
                if (month != null)
                {
                    filter = filter.Where(w => w.Month == month);
                }
                if (indicatorId != null)
                {
                    filter = filter.Where(w => w.IndicatorId == indicatorId);
                }
                if (ageGroupId != null)
                {
                    filter = filter.Where(w => w.AgeGroupId == ageGroupId);
                }
                if (genderId != null)
                {
                    filter = filter.Where(w => w.GenderId == genderId);
                }
                if (keyPopulationId != null)
                {
                    filter = filter.Where(w => w.KeyPopulationId == keyPopulationId);
                }
                if (siteId != null)
                {
                    filter = filter.Where(w => w.SiteId == siteId);
                }
                else if (districId != null)
                {
                    var sites = _dbContext.Sites.Where(s => s.DistrictId == districId).Select(s => s.Id);
                    filter = filter.Where(w => sites.Any(a => a == w.SiteId));
                }
                else if (provinceId != null)
                {
                    var districs = _dbContext.Districts.Where(s => s.ProvinceId == provinceId).Select(s => s.Id);
                    var sites = _dbContext.Sites.Where(s => districs.Any(d => d == s.DistrictId)).Select(s => s.Id);
                    filter = filter.Where(w => sites.Any(s => s ==w.SiteId));
                }
                if (indicatorGroupId != null)
                {
                    filter = filter.Where(w => w.Indicator.IndicatorGroupId == indicatorGroupId);
                }
                var data = filter.OrderBy(s => s.Year).ThenBy(s => s.Quarter).ThenBy(s => s.Month).ThenBy(s => s.Indicator)
                    .Skip((int)(pageIndex * pageSize)).Take((int)pageSize).ToList();
                using (var ms = new MemoryStream())
                {
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet excelSheet = workbook.CreateSheet("Sheet 1");
                    int rowIndex = 0;
                    IRow row = excelSheet.CreateRow(rowIndex++);

                    //
                    ICellStyle titleStyle = workbook.CreateCellStyle();
                    titleStyle.Alignment = HorizontalAlignment.Center;
                    IFont titleFont = workbook.CreateFont();
                    titleFont.IsBold = true;
                    //
                    int cellIndex = 0;
                    titleStyle.SetFont(titleFont);

                    var cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Kỳ báo cáo/Period type");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Năm/Year");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Quý/Quarter");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Tháng/Month");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Mã chỉ số/Indicator code");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Tên chỉ số/Indicator name");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Mã cơ sở/Site code");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Tên cơ sở/Site name");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Nhóm tuổi/Age group");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Giới tính/Gender");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Nhóm nguy cơ/Key population");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Tên thuốc/Drug name");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Đơn vị tính/Drug unit name");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Nguồn dữ liệu/Data source");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Tử số/Numerator");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Mẫu số/Denominator");
                    cell.CellStyle = titleStyle;

                    cell = row.CreateCell(cellIndex++);
                    cell.SetCellValue("Ngày báo cáo/Created Date");
                    cell.CellStyle = titleStyle;

                    data.ForEach(item =>
                    {
                        var row = excelSheet.CreateRow(rowIndex++);
                        row.CreateCell(0).SetCellValue(item.PeriodType);
                        row.CreateCell(1).SetCellValue(item.Year);
                        row.CreateCell(2).SetCellValue(item.Quarter != null ? item.Quarter.Value.ToString() : "");
                        row.CreateCell(3).SetCellValue(item.Month != null ? item.Month.Value.ToString() : "");
                        row.CreateCell(4).SetCellValue(item.Indicator.Code);
                        row.CreateCell(5).SetCellValue(item.Indicator.Name);
                        row.CreateCell(6).SetCellValue(item.Site.Code);
                        row.CreateCell(7).SetCellValue(item.Site.Name);
                        row.CreateCell(8).SetCellValue(item.AgeGroup.Name);
                        row.CreateCell(9).SetCellValue(item.Gender.Name);
                        row.CreateCell(10).SetCellValue(item.KeyPopulation.Name);
                        row.CreateCell(11).SetCellValue(item.DrugName);
                        row.CreateCell(12).SetCellValue(item.DrugUnitName);
                        row.CreateCell(13).SetCellValue(item.DataSource);
                        row.CreateCell(14).SetCellValue(item.Numerator);
                        row.CreateCell(15).SetCellValue(item.Denominator);
                        row.CreateCell(16).SetCellValue(item.DateCreated?.ToString());
                    });
                    workbook.Write(ms);
                    result.Data = ms.ToArray() as byte[];
                    result.Succeed = true;
                }

            }
            catch (Exception ex)
            {
                result.Succeed = false;
                result.Error.ErrorMessage = ex.Message;
            }
            return result;
        }
    }
}
