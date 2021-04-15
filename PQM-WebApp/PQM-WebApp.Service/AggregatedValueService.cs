using Mapster;
using Microsoft.EntityFrameworkCore;
using Nest;
using static Nest.Infer;
using PQM_WebApp.Data;
using PQM_WebApp.Data.Entities;
using PQM_WebApp.Data.Enums;
using PQM_WebApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using PQM_WebApp.Data.ViewModels;

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
        ResultModel GetAggregatedValues(int year, int? quarter, int? month, string indicatorGroup, string indicatorCode, string groupBy, string provinceCode, string districtCode);
        ResultModel GetSixMonthAggregatedValues(int year, int month, string indicatorGroup, string indicatorCode, string groupBy, string provinceCode, string districtCode);
        ResultModel PopulateData(string indicator, int year, int month, int? day = null, bool all = false);
        ResultModel GetChartData(string indicator, int year, int quater, string provinceCode, string districtCode, int? month = null, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null);
        ResultModel GetIndicatorValues(string provinceCode, string districtCode, string indicatorGroup, string indicatorCode, int year
            , int? quarter = null, int? month = null, string ageGroups = "", string keyPopulations = "", string genders = "", string sites = "");
    }

    public class AggregatedValueService : IAggregatedValueService
    {
        private const string INDICATOR_VALUE = nameof(IndicatorElasticModel.Value);
        private const string INDICATOR_DENOMINATOR = nameof(IndicatorElasticModel.Denominator);
        private const string INDICATOR_NUMERATOR = nameof(IndicatorElasticModel.Numerator);

        private readonly AppDBContext _dBContext;
        private readonly ElasticClient _elasticClient;

        public AggregatedValueService(AppDBContext dBContext, ElasticClient elasticClient)
        {
            _dBContext = dBContext;
            _elasticClient = elasticClient;
        }

        public ResultModel GetAggregatedValues(int year, int? quarter, int? month, string indicatorGroup, string indicatorCode, string groupBy, string provinceCode, string districtCode)
        {
            var result = new ResultModel();
            result.Succeed = true;
            try
            {
                var dims = groupBy == "AgeGroup" ? _dBContext.AgeGroups.ToList().Select(s => s.Adapt<DimensionViewModel>())
                    : groupBy == "KeyPopulation" ? _dBContext.KeyPopulations.ToList().Select(s => s.Adapt<DimensionViewModel>())
                    : groupBy == "Gender" ? _dBContext.Sex.ToList().Select(s => s.Adapt<DimensionViewModel>())
                    : _dBContext.Sites.ToList().Select(s => s.Adapt<DimensionViewModel>());
                
                var _districts = !string.IsNullOrEmpty(districtCode) ? districtCode.Split(',').ToList() : new List<string>();
                var response = SearchOnElastic(provinceCode: provinceCode, districts: _districts
                    , indicatorGroup: indicatorGroup, indicatorCode: indicatorCode
                    , year: year, quarter: quarter, month: month, groupBy: groupBy, onlyTotal: true);
                var data = response.Aggregations.Terms(groupBy).Buckets.Select(s => new IndicatorModel
                {
                    Name = s.Key,
                    Value = new IndicatorValue
                    {
                        Value = (int)s.Sum(INDICATOR_VALUE).Value,
                        Numerator = (int)s.Sum(INDICATOR_NUMERATOR).Value,
                        Denominator = (int)s.Sum(INDICATOR_DENOMINATOR).Value,
                    }
                }).ToList();
                var dimData = new List<DimensionViewModel>();
                foreach (var item in data)
                {
                    var dim = dims.FirstOrDefault(s => s.Name == item.Name);
                    dimData.Add(new DimensionViewModel
                    {
                        Id = dim.Id,
                        Name = dim.Name,
                        Code = dim.Code,
                        Value = item.Value.Value,
                        Numerator = item.Value.Numerator,
                        Denominator = item.Value.Denominator,
                        Order = dim.Order,
                    });
                }
                result.Data = dimData.OrderBy(s => s.Order);
                return result;
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
            }).OrderBy(o => o.Order).Select(s => new
            {
                s.Name,
                Value = s.Value.DataType == DataType.Number ? s.Value.Value : (double)s.Value.Numerator / s.Value.Denominator,
                s.Value.DataType,
            }).ToList();
            var skip = data.Count > 6 ? data.Count - 6 : 0;
            return new ResultModel()
            {
                Succeed = true,
                Data = data.Skip(skip),
            };
        }

        private QueryContainer BuildLocationQuery(string provinceCode, List<string> districts, List<string> sites = null)
        {
            if (sites != null && sites.Count > 0)
            {
                return (new QueryContainerDescriptor<IndicatorElasticModel>()).Terms(t => t.Field(f => f.Site).Terms(sites));
            }
            if (districts.Count > 0)
            {
                return (new QueryContainerDescriptor<IndicatorElasticModel>()).Terms(t => t.Field(f => f.DistrictCode).Terms(districts));
            }
            if (!string.IsNullOrEmpty(provinceCode))
            {
                return (new QueryContainerDescriptor<IndicatorElasticModel>()).Match(m => m.Field(f => f.ProvinceCode).Query(provinceCode));
            }
            return null;
        }

        private Dictionary<string, IAggregationContainer> BuildAggreagtionQuery(string groupBy)
        {
            var aggregations = new Dictionary<string, IAggregationContainer>();
            var sumAggregations = new Dictionary<string, IAggregationContainer>();
            sumAggregations.Add(INDICATOR_VALUE, new AggregationContainer { Sum = new SumAggregation(INDICATOR_VALUE, Field<IndicatorElasticModel>(p => p.Value)) });
            sumAggregations.Add(INDICATOR_NUMERATOR, new AggregationContainer { Sum = new SumAggregation(INDICATOR_NUMERATOR, Field<IndicatorElasticModel>(p => p.Numerator)) });
            sumAggregations.Add(INDICATOR_DENOMINATOR, new AggregationContainer { Sum = new SumAggregation(INDICATOR_DENOMINATOR, Field<IndicatorElasticModel>(p => p.Denominator)) });
            TermsAggregation termAggregation;
            switch (groupBy)
            {
                case ("IndicatorCode"):
                    termAggregation = new TermsAggregation(groupBy)
                    {
                        Field = Field<IndicatorElasticModel>(p => p.IndicatorCode.Suffix("keyword")),
                        Size = 100,
                    };
                    aggregations.Add(groupBy, new AggregationContainer { Terms = termAggregation, Aggregations = sumAggregations });
                    return aggregations;
                case ("AgeGroup"):
                    termAggregation = new TermsAggregation(groupBy)
                    {
                        Field = Field<IndicatorElasticModel>(p => p.AgeGroup.Suffix("keyword")),
                        Size = 100,
                    };
                    aggregations.Add(groupBy, new AggregationContainer { Terms = termAggregation, Aggregations = sumAggregations });
                    return aggregations;
                case ("KeyPopulation"):
                    termAggregation = new TermsAggregation(groupBy)
                    {
                        Field = Field<IndicatorElasticModel>(p => p.KeyPopulation.Suffix("keyword")),
                        Size = 100,
                    };
                    aggregations.Add(groupBy, new AggregationContainer { Terms = termAggregation, Aggregations = sumAggregations });
                    return aggregations;
                case ("Gender"):
                    termAggregation = new TermsAggregation(groupBy)
                    {
                        Field = Field<IndicatorElasticModel>(p => p.Gender.Suffix("keyword")),
                        Size = 100,
                    };
                    aggregations.Add(groupBy, new AggregationContainer { Terms = termAggregation, Aggregations = sumAggregations });
                    return aggregations;
                case ("Site"):
                    termAggregation = new TermsAggregation(groupBy)
                    {
                        Field = Field<IndicatorElasticModel>(p => p.Site.Suffix("keyword")),
                        Size = 100,
                    };
                    aggregations.Add(groupBy, new AggregationContainer { Terms = termAggregation, Aggregations = sumAggregations });
                    return aggregations;
                default:
                    return aggregations;
            }
        }

        private ISearchResponse<IndicatorElasticModel> SearchOnElastic(string provinceCode, List<string> districts
            , string indicatorGroup, string indicatorCode
            , int year , int? quarter = null, int? month = null
            , List<string> ageGroups = null, List<string> keyPopulations = null, List<string> genders = null, List<string> sites = null
            , string groupBy = "", bool onlyTotal = false)
        {

            var queryContainers = new List<QueryContainer>();
            var filterContainers = new List<QueryContainer>();
            var shouldContainers = new List<QueryContainer>();
            //year
            queryContainers.Add((new QueryContainerDescriptor<IndicatorElasticModel>()).Match(m => m.Field(f => f.Year).Query(year.ToString())));
            if (month != null)
            {
                queryContainers.Add((new QueryContainerDescriptor<IndicatorElasticModel>()).Match(m => m.Field(f => f.Month).Query(month.ToString())));
            }
            else if (quarter != null)
            {
                queryContainers.Add((new QueryContainerDescriptor<IndicatorElasticModel>()).Match(m => m.Field(f => f.Quarter).Query(quarter.ToString())));
            }
            if (!string.IsNullOrEmpty(indicatorCode))
            {
                queryContainers.Add((new QueryContainerDescriptor<IndicatorElasticModel>()).Match(m => m.Field(f => f.IndicatorCode).Query(indicatorCode)));
            }
            if (!string.IsNullOrEmpty(indicatorGroup))
            {
                queryContainers.Add((new QueryContainerDescriptor<IndicatorElasticModel>()).Match(m => m.Field(f => f.IndicatorGroup).Query(indicatorGroup)));
            }
            var locationQuery = BuildLocationQuery(provinceCode, districts, sites);
            if (locationQuery != null)
            {
                queryContainers.Add(locationQuery);
            }
            if (ageGroups != null && ageGroups.Count > 0)
            {
                queryContainers.Add((new QueryContainerDescriptor<IndicatorElasticModel>()).Terms(t => t.Field(f => f.AgeGroup).Terms(ageGroups)));
            }
            if (keyPopulations != null && keyPopulations.Count > 0)
            {
                queryContainers.Add((new QueryContainerDescriptor<IndicatorElasticModel>()).Terms(t => t.Field(f => f.KeyPopulation).Terms(keyPopulations)));
            }
            if (genders != null && genders.Count > 0)
            {
                queryContainers.Add((new QueryContainerDescriptor<IndicatorElasticModel>()).Terms(t => t.Field(f => f.KeyPopulation).Terms(genders)));
            }
            if (onlyTotal)
            {
                queryContainers.Add((new QueryContainerDescriptor<IndicatorElasticModel>()).Match(m => m.Field(f => f.IsTotal).Query("true")));
            }
            var searchRequest = new SearchRequest<IndicatorElasticModel>("indicatorvalue")
            {
                From = 0,
                Size = 1,
                Query = new BoolQuery { Must = queryContainers, Filter = filterContainers, Should = shouldContainers },
                Aggregations = BuildAggreagtionQuery(groupBy),
            };
            return _elasticClient.Search<IndicatorElasticModel>(searchRequest);
        }

        public ResultModel GetIndicatorValues(string provinceCode, string districtCode, string indicatorGroup, string indicatorCode
            , int year, int? quarter = null, int? month = null
            , string ageGroups = "", string keyPopulations = "", string genders = "", string sites = "")
        {
            var _districts = !string.IsNullOrEmpty(districtCode) ? districtCode.Split(',').ToList() : new List<string>();
            var _ageGroups = !string.IsNullOrEmpty(ageGroups) ? ageGroups.Split(',').ToList() : new List<string>();
            var _keyPopulations = !string.IsNullOrEmpty(keyPopulations) ? keyPopulations.Split(',').ToList() : new List<string>();
            var _genders = !string.IsNullOrEmpty(genders) ? genders.Split(',').ToList() : new List<string>();
            var _sites = !string.IsNullOrEmpty(sites) ? sites.Split(',').ToList() : new List<string>();

            var indicators = _dBContext.Indicators.Include(i => i.IndicatorGroup).ToList();
            quarter = month == null ? quarter : null;
            //old data
            var preYear = quarter != null ? (quarter > 1 ? year : year - 1) : month != null ? (month > 1 ? year : year - 1) : year - 1;
            int? preMonth = month != null ? (month > 1 ? month - 1 : 12) : null;
            int? preQuarter = quarter != null ? (quarter > 1 ? quarter - 1 : 4) : null;
            var response2 = SearchOnElastic(provinceCode: provinceCode, districts: _districts, indicatorGroup: indicatorGroup, indicatorCode: indicatorCode
                , year: preYear, quarter: preQuarter, month: preMonth
                , ageGroups: _ageGroups, keyPopulations: _keyPopulations, genders: _genders, sites: _sites
                , groupBy: "IndicatorCode");
            var predata = response2.Aggregations.Terms("IndicatorCode").Buckets.Select(s => new IndicatorModel
            {
                Name = s.Key,
                Value = new IndicatorValue
                {
                    Value = (int)s.Sum(INDICATOR_VALUE).Value,
                    Numerator = (int)s.Sum(INDICATOR_NUMERATOR).Value,
                    Denominator = (int)s.Sum(INDICATOR_DENOMINATOR).Value,
                }
            }).ToList();
            predata.ForEach(s =>
            {
                var indicator = indicators.FirstOrDefault(f => f.Code == s.Name);
                s.Name = indicator.Name;
                s.Group = indicator.IndicatorGroup.Name;
                s.Order = indicator.Order;
                s.Value.DataType = s.Value.Value > 0 ? DataType.Number : DataType.Percent;
            });

            //new data
            var response = SearchOnElastic(provinceCode: provinceCode, districts: _districts, indicatorGroup: indicatorGroup, indicatorCode: indicatorCode
                , year: year, quarter: quarter, month: month
                , ageGroups: _ageGroups, keyPopulations: _keyPopulations, genders: _genders, sites: _sites
                , groupBy: "IndicatorCode");
            var data = response.Aggregations.Terms("IndicatorCode").Buckets.Select(s => new IndicatorModel
            {
                Name = s.Key,
                Value = new IndicatorValue
                {
                    Value = (int)s.Sum(INDICATOR_VALUE).Value,
                    Numerator = (int)s.Sum(INDICATOR_NUMERATOR).Value,
                    Denominator = (int)s.Sum(INDICATOR_DENOMINATOR).Value,
                }
            }).ToList();
            data.ForEach(s =>
            {
                var indicator = indicators.FirstOrDefault(f => f.Code == s.Name);
                s.Name = indicator.Name;
                s.Group = indicator.IndicatorGroup.Name;
                s.Order = indicator.Order;
                s.Value.DataType = s.Value.Value > 0 ? DataType.Number : DataType.Percent;
                var pre = predata.FirstOrDefault(f => f.Name == s.Name);
                if (pre != null)
                {
                    var p = s.Value.DataType == DataType.Number
                                                        ? (double)(s.Value.Value - pre.Value.Value) / pre.Value.Value
                                                        : (double)(s.Value.Numerator * pre.Value.Denominator
                                                                    - pre.Value.Numerator * s.Value.Denominator)
                                                                    / (double)(s.Value.Denominator * pre.Value.Denominator);
                    s.Trend = new IndicatorTrend
                    {
                        ComparePercent = p >= 0 ? p : -p,
                        Direction = p >= 0 ? 1 : -1,
                    };
                }
            });
            return new ResultModel()
            {
                Data = data.OrderBy(s => s.Order).ToList(),
                Succeed = true,
            };
        }

        public ResultModel GetSixMonthAggregatedValues(int year, int month, string indicatorGroup, string indicatorCode, string groupBy, string provinceCode, string districtCode)
        {
            throw new NotImplementedException();
        }

        public ResultModel PopulateData(string indicator, int year, int month, int? day = null, bool all = false)
        {
            var data = _dBContext.AggregatedValues
                                 .Where(w => all || (w.Month.MonthNumOfYear == month && w.Month.Year.Year == year && w.Indicator.Name == indicator))
                                 .Select(s => new IndicatorElasticModel
                                 {
                                     IndicatorName = s.Indicator.Name,
                                     IndicatorCode = s.Indicator.Code,
                                     IndicatorGroup = s.Indicator.IndicatorGroup.Name,
                                     IsTotal = s.Indicator.IsTotal.Value,
                                     Quarter = s.Month.MonthNumOfYear <= 3 ? 1 : s.Month.MonthNumOfYear <= 6 ? 2 : s.Month.MonthNumOfYear <= 9 ? 3 : 4,
                                     DistrictCode = s.Site.District.Code,
                                     ProvinceCode = s.Site.District.Province.Code,
                                     ValueType = s.DataType == DataType.Number ? 1 : 2,
                                     Value = s.Value,
                                     Denominator = s.Denominator,
                                     Numerator = s.Numerator,
                                     AgeGroup = s.AgeGroup.Name,
                                     KeyPopulation = s.KeyPopulation.Name,
                                     Site = s.Site.Name,
                                     Gender = s.Sex.Name,
                                     Month = !all ? month : s.Month.MonthNumOfYear,
                                     Year = !all ? year : s.Month.Year.Year,
                                     Day = !all ? day : s.Date.Date.Day,
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
