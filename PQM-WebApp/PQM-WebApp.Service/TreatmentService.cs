using PQM_WebApp.Data;
using PQM_WebApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PQM_WebApp.Service
{
    public interface ITreatmentService
    {
        ResultModel GetIndicators(int year, int quater, int? month, string provinceCode, string districtCode, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null);

    }

    public class TreatmentService : ITreatmentService
    {
        private AppDBContext _dbContext { get; set; }

        public TreatmentService(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultModel GetIndicators(int year, int quater, int? month, string provinceCode, string districtCode, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null)
        {
            var fromMonth = quater == 1 ? 1 : quater == 2 ? 4 : quater == 3 ? 7 : 10;
            var toMonth = quater == 1 ? 3 : quater == 2 ? 6 : quater == 3 ? 9 : 12;
            var months = _dbContext.DimMonths.Where(m => m.Year.Year == year && (fromMonth <= m.MonthNumOfYear && m.MonthNumOfYear <= toMonth) && (month == null || m.MonthNumOfYear == month)).Select(m => m.Id);
            var districts = _dbContext.Districts.Where(d => d.Province.Code == provinceCode && (string.IsNullOrEmpty(districtCode) || d.Code == districtCode)).Select(s => s.Id);
            var sites = _dbContext.Sites.Where(s => districts.Contains(s.DistrictId)).Select(s => s.Id);
            var aggregatedValues = _dbContext.AggregatedValues.Where(w => months.Contains(w.MonthId) && sites.Contains(w.SiteId) && w.Indicator.IndicatorGroup.Name == "Treatment");
            if (!string.IsNullOrEmpty(ageGroups))
            {
                var _ageGroups = ageGroups.Split(',').Select(s => Guid.Parse(s));
                aggregatedValues = aggregatedValues.Where(s => _ageGroups.Contains(s.AgeGroupId));
            }
            if (!string.IsNullOrEmpty(keyPopulations))
            {
                var _keyPopulations = keyPopulations.Split(',').Select(s => Guid.Parse(s));
                aggregatedValues = aggregatedValues.Where(s => _keyPopulations.Contains(s.KeyPopulationId));
            }
            if (!string.IsNullOrEmpty(genders))
            {
                var _genders = genders.Split(',').Select(s => Guid.Parse(s));
                aggregatedValues = aggregatedValues.Where(s => _genders.Contains(s.SexId));
            }
            if (!string.IsNullOrEmpty(clinnics))
            {
                var _clinnics = clinnics.Split(',').Select(s => Guid.Parse(s));
                aggregatedValues = aggregatedValues.Where(s => _clinnics.Contains(s.SiteId));
            }
            var groupIndicator = aggregatedValues.ToList().GroupBy(g => g.Indicator);
            var data = groupIndicator.Select(s => new IndicatorModel
            {
                Order = s.Key.Order,
                Group = s.Key.IndicatorGroup.Name,
                Name = s.Key.Name,
                Value = new IndicatorValue
                {
                    Value = s.Sum(_ => _.Value).Value,
                    Numerator = s.Sum(_ => _.Numerator),
                    Denominator = s.Sum(_ => _.Denominator),
                    DataType = s.FirstOrDefault().DataType,
                    CriticalInfo = "green"
                },
                CriticalInfo = "green",
                Trend = new IndicatorTrend
                {
                    CriticalInfo = "green",
                    ComparePercent = 98,
                    Direction = 1,
                }
            }
            ).OrderBy(o => o.Order).ToList();
            return new ResultModel()
            {
                Succeed = true,
                Data = data,
            };
        }
    }
}
