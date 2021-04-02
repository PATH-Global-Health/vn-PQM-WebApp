using PQM_WebApp.Data;
using PQM_WebApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PQM_WebApp.Service
{
    public interface ITestingService
    {
        ResultModel GetIndicators(int year, int quater, int? month, string provinceCode, string districtCode, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null);
        ResultModel GetHealthMap(int year, int quater, int? month, string provinceCode, string districtCode);
    }

    public class TestingService : ITestingService
    {
        private AppDBContext _dbContext { get; set; }

        public TestingService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }



        public ResultModel GetIndicators(int year, int quater, int? month, string provinceCode, string districtCode, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null)
        {
            var indicatorGroup = "Testing";
            var fromMonth = quater == 1 ? 1 : quater == 2 ? 4 : quater == 3 ? 7 : 10;
            var toMonth = quater == 1 ? 3 : quater == 2 ? 6 : quater == 3 ? 9 : 12;
            var months = _dbContext.DimMonths.Where(m => m.Year.Year == year
                        && (month != null || fromMonth <= m.MonthNumOfYear && m.MonthNumOfYear <= toMonth)
                        && (month == null || m.MonthNumOfYear == month)).Select(m => m.Id);


            var lastQuater = quater - 1 > 0 ? quater - 1 : 4;
            var lastYear = quater - 1 > 0 ? year : year - 1;
            var fromLastMonth = lastQuater == 1 ? 1 : lastQuater == 2 ? 4 : lastQuater == 3 ? 7 : 10;
            var toLastMonth = lastQuater == 1 ? 3 : lastQuater == 2 ? 6 : lastQuater == 3 ? 9 : 12;
            int? lastMonth = month == null ? null : month - 1 > 0 ? month - 1 : 12;
            lastYear = lastMonth == null ? lastYear : month - 1 > 0 ? year : year - 1;
            var lastMonths = _dbContext.DimMonths.Where(m => m.Year.Year == lastYear
                        && (lastMonth != null || fromLastMonth <= m.MonthNumOfYear && m.MonthNumOfYear <= toLastMonth)
                        && (lastMonth == null || m.MonthNumOfYear == lastMonth)).Select(m => m.Id);

            var _districts = !string.IsNullOrEmpty(districtCode) ? districtCode.Split(',') : null;
            var districts = _dbContext.Districts.Where(d => d.Province.Code == provinceCode && (string.IsNullOrEmpty(districtCode) || _districts.Contains(d.Code))).Select(s => s.Id);
            var sites = _dbContext.Sites.Where(s => districts.Contains(s.DistrictId)).Select(s => s.Id);
            var aggregatedValues = _dbContext.AggregatedValues.Where(w => months.Contains(w.MonthId) && sites.Contains(w.SiteId) && w.Indicator.IndicatorGroup.Name == indicatorGroup);
            var lastAggregatedValues = _dbContext.AggregatedValues.Where(w => lastMonths.Contains(w.MonthId) && sites.Contains(w.SiteId) && w.Indicator.IndicatorGroup.Name == indicatorGroup);

            if (!string.IsNullOrEmpty(ageGroups))
            {
                var _ageGroups = ageGroups.Split(',').Select(s => Guid.Parse(s));
                aggregatedValues = aggregatedValues.Where(s => _ageGroups.Contains(s.AgeGroupId));
                lastAggregatedValues = lastAggregatedValues.Where(s => _ageGroups.Contains(s.AgeGroupId));
            }
            if (!string.IsNullOrEmpty(keyPopulations))
            {
                var _keyPopulations = keyPopulations.Split(',').Select(s => Guid.Parse(s));
                aggregatedValues = aggregatedValues.Where(s => _keyPopulations.Contains(s.KeyPopulationId));
                lastAggregatedValues = lastAggregatedValues.Where(s => _keyPopulations.Contains(s.KeyPopulationId));
            }
            if (!string.IsNullOrEmpty(genders))
            {
                var _genders = genders.Split(',').Select(s => Guid.Parse(s));
                aggregatedValues = aggregatedValues.Where(s => _genders.Contains(s.SexId));
                lastAggregatedValues = lastAggregatedValues.Where(s => _genders.Contains(s.SexId));
            }
            if (!string.IsNullOrEmpty(clinnics))
            {
                var _clinnics = clinnics.Split(',').Select(s => Guid.Parse(s));
                aggregatedValues = aggregatedValues.Where(s => _clinnics.Contains(s.SiteId));
                lastAggregatedValues = lastAggregatedValues.Where(s => _clinnics.Contains(s.SiteId));
            }
            var groupIndicator = aggregatedValues.ToList().GroupBy(g => g.Indicator);

            var lastGroupIndicator = lastAggregatedValues.ToList().GroupBy(g => g.Indicator);

            var data = new List<IndicatorModel>();
            foreach (var s in groupIndicator)
            {
                var lastS = lastGroupIndicator.FirstOrDefault(k => k.Key.Name == s.Key.Name);
                var dataType = s.FirstOrDefault().DataType;

                var item = new IndicatorModel
                {
                    Order = s.Key.Order,
                    Group = s.Key.IndicatorGroup.Name,
                    Name = s.Key.Name,
                    Value = new IndicatorValue
                    {
                        Value = s.Sum(_ => _.Value).Value,
                        Numerator = s.Sum(_ => _.Numerator),
                        Denominator = s.Sum(_ => _.Denominator),
                        DataType = dataType,
                        CriticalInfo = "green"
                    },
                    CriticalInfo = "green",
                    Trend = new IndicatorTrend
                    {
                        ComparePercent = 0,
                        CriticalInfo = null,
                        Direction = 0,
                    }
                };

                if (lastS != null)
                {
                    var lastItem = new IndicatorModel
                    {
                        Order = lastS.Key.Order,
                        Group = lastS.Key.IndicatorGroup.Name,
                        Name = lastS.Key.Name,
                        Value = new IndicatorValue
                        {
                            Value = lastS.Sum(_ => _.Value).Value,
                            Numerator = lastS.Sum(_ => _.Numerator),
                            Denominator = lastS.Sum(_ => _.Denominator),
                            DataType = dataType,
                            CriticalInfo = "green"
                        },
                    };
                    if (dataType == Data.Enums.DataType.Number)
                    {
                        var p = ((double)item.Value.Value - lastItem.Value.Value) / lastItem.Value.Value;
                        item.Trend.ComparePercent = p >= 0 ? Math.Round(p * 100, 2) : Math.Round(-(p * 100), 2);
                        item.Trend.CriticalInfo = p > 0 ? "green" : "red";
                        item.Trend.Direction = p > 0 ? 1 : -1;
                    }
                    else
                    {
                        var p1 = (double)item.Value.Numerator / item.Value.Denominator;
                        var p2 = (double)lastItem.Value.Numerator / lastItem.Value.Denominator;
                        var p = (p1 - p2) / p2;
                        item.Trend.ComparePercent = p >= 0 ? Math.Round((p.Value * 100), 2) : Math.Round(-(p.Value * 100), 2);
                        item.Trend.CriticalInfo = p > 0 ? "green" : "red";
                        item.Trend.Direction = p > 0 ? 1 : -1;
                    }
                }

                data.Add(item);
            }

            data = data.OrderBy(s => s.Order).ToList();

            return new ResultModel()
            {
                Succeed = true,
                Data = data,
            };
        }

        public ResultModel GetHealthMap(int year, int quarter, int? month, string provinceCode, string districtCode)
        {
            try
            {
                var indicatorName = "HTS_TST_Recency";

                var fromMonth = quarter == 1 ? 1 : quarter == 2 ? 4 : quarter == 3 ? 7 : 10;
                var toMonth = quarter == 1 ? 3 : quarter == 2 ? 6 : quarter == 3 ? 9 : 12;
                var months = _dbContext.DimMonths.Where(m => m.Year.Year == year
                            && (month != null || fromMonth <= m.MonthNumOfYear && m.MonthNumOfYear <= toMonth)
                            && (month == null || m.MonthNumOfYear == month)).Select(m => m.Id);
                var districts = _dbContext.Districts.Where(d => d.Province.Code == provinceCode && (string.IsNullOrEmpty(districtCode) || d.Code == districtCode)).Select(s => s.Id);
                var sites = _dbContext.Sites.Where(s => districts.Contains(s.DistrictId)).Select(s => s.Id);
                var aggregatedValues = _dbContext.AggregatedValues.Where(w => months.Contains(w.MonthId)
                                                                           && sites.Contains(w.SiteId)
                                                                           && w.Indicator.Name == indicatorName).ToList();
                var rs = aggregatedValues.GroupBy(f => f.Site.District)
                    .Select(s => new
                    {
                        Lat = s.Key.Lat,
                        Lon = s.Key.Lng,
                        Count = s.Sum(f => f.Value),
                    })
                    .ToList();
                return new ResultModel()
                {
                    Succeed = true,
                    Data = rs,
                };
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    Succeed = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
