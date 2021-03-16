using Mapster;
using Microsoft.EntityFrameworkCore;
using Nest;
using PQM_WebApp.Data;
using PQM_WebApp.Data.Entities;
using PQM_WebApp.Data.Enums;
using PQM_WebApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PQM_WebApp.Service
{
    public interface IAggregatedValueService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="year">Năm</param>
        /// <param name="month">Tháng</param>
        /// <param name="quarter">Quý</param>
        /// <param name="indicatorGroup">Loại</param>
        /// <param name="indicatorCode">Mã indicator</param>
        /// <param name="groupBy">Loại</param>
        /// <returns>Loại và Tổng giá trị</returns>
        ResultModel GetAggregatedValues(int year, int quarter, int? month, string indicatorGroup, string indicatorCode, string groupBy, string provinceCode, string districtCode);
        ResultModel GetSixMonthAggregatedValues(int year, int month, string indicatorGroup, string indicatorCode, string groupBy, string provinceCode, string districtCode);
        ResultModel PopulateData(string indicator, int year, int month, int? day = null);
        ResultModel GetChartData(string indicator, int year, int quater, string provinceCode, string districtCode, int? month = null, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null);
    }

    public class AggregatedValueService : IAggregatedValueService
    {
        private readonly AppDBContext _dBContext;
        private readonly ElasticClient _elasticClient;

        public AggregatedValueService(AppDBContext dBContext, ElasticClient elasticClient)
        {
            _dBContext = dBContext;
            _elasticClient = elasticClient;
        }

        public ResultModel GetAggregatedValues(int year, int quarter, int? month, string indicatorGroup, string indicatorCode, string groupBy, string provinceCode, string districtCode)
        {
            var result = new ResultModel();
            try
            {
                var _districts = !string.IsNullOrEmpty(districtCode) ? districtCode.Split(',') : null;
                var districts = _dBContext.Districts.Where(d => d.Province.Code == provinceCode && (string.IsNullOrEmpty(districtCode) || _districts.Contains(d.Code))).Select(s => s.Id);
                var sites = _dBContext.Sites.Where(s => districts.Contains(s.DistrictId)).Select(s => s.Id);
                var fromMonth = quarter == 1 ? 1 : quarter == 2 ? 4 : quarter == 3 ? 7 : 10;
                var toMonth = quarter == 1 ? 3 : quarter == 2 ? 6 : quarter == 3 ? 9 : 12;
                var dimension = groupBy.Adapt<DimensionGroupType>();
                var dimensionString = dimension.Adapt<string>();
                var filter = _dBContext.AggregatedValues
                                        .Where(_ => (month == null ||_.Month.MonthNumOfYear == month)
                                        && _.Month.Year.Year == year
                                        && (fromMonth <= _.Month.MonthNumOfYear && _.Month.MonthNumOfYear <= toMonth)
                                        && (string.IsNullOrEmpty(indicatorGroup) || _.Indicator.IndicatorGroup.Name == indicatorGroup)
                                        && (string.IsNullOrEmpty(indicatorCode) || _.Indicator.Name == indicatorCode || _.Indicator.Code == indicatorCode)
                                        && sites.Contains(_.SiteId))
                                        .Include(_ => dimensionString)
                                        .ToList();
                var grouped = filter.GroupBy(_ => _.GetType().GetProperty(dimensionString).GetValue(_, null))
                                    .OrderBy(_ => (_.Key as DimensionGroup).Order)
                                    .Select(s => new {
                                                    (s.Key as DimensionGroup).Id,
                                                    (s.Key as DimensionGroup).Name,
                                                    Code = groupBy == "Site" ? (s.Key as Site).Code : "",
                                                    Value = s.Sum(v => v.Value),
                                                    Numerator = s.Sum(n => n.Numerator),
                                                    Denominator = s.Sum(d => d.Denominator)
                                                }
                                    ).Where(d => d.Value != 0).ToList();
                result.Data = grouped;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
        }

        public ResultModel GetChartData(string indicator, int year, int quater, string provinceCode, string districtCode, int? month = null, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null)
        {
            var toMonth = quater == 1 ? 3 : quater == 2 ? 6 : quater == 3 ? 9 : 12;
            var _districts = !string.IsNullOrEmpty(districtCode) ? districtCode.Split(',') : null;
            var districts = _dBContext.Districts.Where(d => d.Province.Code == provinceCode && (string.IsNullOrEmpty(districtCode) || _districts.Contains(d.Code))).Select(s => s.Id);
            var sites = _dBContext.Sites.Where(s => districts.Contains(s.DistrictId)).Select(s => s.Id);
            var limit = month == null ? year * 100 + toMonth : year * 100 + month;
            var months = _dBContext.DimMonths.Where(w => (w.Year.Year * 100 + w.MonthNumOfYear) <= limit).Select(s => s.Id);
            var aggregatedValues = _dBContext.AggregatedValues.Where(w => months.Contains(w.MonthId) && sites.Contains(w.SiteId) && w.Indicator.Name == indicator);
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
            var groupIndicator = aggregatedValues.ToList().GroupBy(g => g.Month);
            var data = groupIndicator.Select(s => new IndicatorModel
            {
                Order = s.Key.MonthNumOfYear + s.Key.Year.Year * 100,
                Group = s.Key.MonthNumOfYear + "-" + s.Key.Year.Year,
                Name = s.Key.MonthNumOfYear + "-" + s.Key.Year.Year,
                Value = new IndicatorValue
                {
                    Value = s.Sum(_ => _.Value).Value,
                    Numerator = s.Sum(_ => _.Numerator),
                    Denominator = s.Sum(_ => _.Denominator),
                    DataType = s.FirstOrDefault().DataType,
                },
            }
            ).OrderBy(o => o.Order).Select(s => new {
                s.Name,
                Value = s.Value.DataType == DataType.Number ? s.Value.Value : (double)s.Value.Numerator / s.Value.Denominator,
            }).ToList();
            return new ResultModel()
            {
                Succeed = true,
                Data = data,
            };
        }

        public ResultModel GetSixMonthAggregatedValues(int year, int month, string indicatorGroup, string indicatorCode, string groupBy, string provinceCode, string districtCode)
        {
            throw new NotImplementedException();
        }

        public ResultModel PopulateData(string indicator, int year, int month, int? day = null)
        {
            var data = _dBContext.AggregatedValues
                                 .Where(w => w.Month.MonthNumOfYear == month && w.Month.Year.Year == year && w.Indicator.Name == indicator)
                                 .Select(s => new IndicatorElasticModel {
                                    Name = s.Indicator.Name,
                                    ValueType = s.DataType == DataType.Number ? 1 : 2,
                                    Value = s.Value,
                                    Denominator = s.Denominator,
                                    Numerator = s.Numerator,
                                    AgeGroup = s.AgeGroup.Name,
                                    KeyPopulation =s.KeyPopulation.Name,
                                    Site = s.Site.Name,
                                    Gender = s.Sex.Name,
                                    Month = month,
                                    Year = year,
                                    Day = day,
                                 }).ToList();
            var response = _elasticClient.IndexMany(data, "indicatorvalue");

            return new ResultModel()
            {
                Succeed = true,
            };
        }

        private int TryParse(string value)
        {
            int result = 0;
            try
            {
                result = int.Parse(value);
            }
            catch (Exception) { }
            return result;
        }
    }
}
