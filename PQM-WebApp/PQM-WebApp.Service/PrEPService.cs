using PQM_WebApp.Data;
using PQM_WebApp.Data.Enums;
using PQM_WebApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PQM_WebApp.Service
{
    public interface IPrEPService
    {
        ResultModel GetIndicators(int year, int quater, int? month, string provinceCode, string districtCode);
        ResultModel GetIndicator(string name);
    }

    public class PrEPService : IPrEPService
    {
        private AppDBContext _dbContext { get; set; }
        public PrEPService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel GetIndicator(string name)
        {
            var data = new List<IndicatorModel>()
                {
                    new IndicatorModel()
                    {
                        Group = "PrEP",
                        Name = "PrEP_NEW",
                        Value = new IndicatorValue()
                        {
                            Value = 1276,
                            DataType = DataType.Number,
                            CriticalInfo = "red",
                        },
                        CriticalInfo = null,
                        Trend = new IndicatorTrend()
                        {
                            Direction  = -1,
                            CriticalInfo = "green",
                            ComparePercent = 95,
                        }
                    },
                    new IndicatorModel()
                    {
                        Group = "PrEP",
                        Name = "PrEP_CURR",
                        Value = new IndicatorValue()
                        {
                            Value = 1925,
                            DataType = DataType.Number,
                            CriticalInfo = "green",
                        },
                        CriticalInfo = null,
                        Trend = new IndicatorTrend()
                        {
                            Direction  = 1,
                            CriticalInfo = "green",
                            ComparePercent = 110,
                        }
                    },
                    new IndicatorModel()
                    {
                        Group = "PrEP",
                        Name = "%PrEP_3M",
                        Value = new IndicatorValue()
                        {
                            Value = 0,
                            Numerator = 92,
                            Denominator = 100,
                            DataType = DataType.Percent,
                            CriticalInfo = "green",
                        },
                        CriticalInfo = null,
                        Trend = new IndicatorTrend()
                        {
                            Direction  = 1,
                            CriticalInfo = "green",
                            ComparePercent = 125
                        }
                    }
                };
            var indicator = data.FirstOrDefault(d => d.Name == name);
            return new ResultModel()
            {
                Succeed = true,
                Data = indicator,
            };
        }

        public ResultModel GetIndicators(int year, int quater, int? month, string provinceCode, string districtCode)
        {
            var fromMonth = quater == 1 ? 1 : quater == 2 ? 4 : quater == 3 ? 7 : 10;
            var toMonth = quater == 1 ? 3 : quater == 2 ? 6 : quater == 3 ? 9 : 12;
            var months = _dbContext.DimMonths.Where(m => m.Year.Year == year && (fromMonth <= m.MonthNumOfYear && m.MonthNumOfYear <= toMonth) && (month == null || m.MonthNumOfYear == month)).Select(m => m.Id);
            var districts = _dbContext.Districts.Where(d => d.Province.Code == provinceCode && (string.IsNullOrEmpty(districtCode) || d.Code == districtCode)).Select(s => s.Id);
            var sites = _dbContext.Sites.Where(s => districts.Contains(s.DistrictId)).Select(s => s.Id);
            var aggregatedValues = _dbContext.AggregatedValues.Where(w => months.Contains(w.MonthId) && sites.Contains(w.SiteId) && w.Indicator.IndicatorGroup.Name == "PrEP");
            var groupIndicator = aggregatedValues.ToList().GroupBy(g => g.Indicator);
            var data = groupIndicator.Select(s => new IndicatorModel
                                                {
                                                    Group = "PrEP",
                                                    Name = s.Key.Name,
                                                    Value = new IndicatorValue { 
                                                        Value = s.Sum(_ => _.Value).Value,
                                                        Numerator = s.Sum(_ => _.Numerator),
                                                        Denominator = s.Sum(_ => _.Denominator),
                                                        DataType = s.FirstOrDefault().DataType,
                                                    },
                                                    CriticalInfo = "green",
                                                    Trend = new IndicatorTrend
                                                    {
                                                        CriticalInfo = "green",
                                                        ComparePercent = 98,
                                                        Direction = 1,
                                                    }
                                                }
            ).ToList();
            return new ResultModel()
            {
                Succeed = true,
                Data = data,
            };
        }
    }
}
