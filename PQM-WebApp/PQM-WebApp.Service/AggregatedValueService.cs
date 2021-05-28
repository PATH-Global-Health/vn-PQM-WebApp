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
using Microsoft.AspNetCore.Http;
using System.IO;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using PQM_WebApp.Data.Extensions;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using PQM_WebApp.Service.Utils;

namespace PQM_WebApp.Service
{
    public interface IAggregatedValueService
    {
        ResultModel Get(int? pageIndex = 0, int? pageSize = int.MaxValue
                      , string period = null, int? year = null, int? quarter = null, int? month = null
                      , Guid? indicatorId = null, Guid? ageGroupId = null, Guid? genderId = null, Guid? keyPopulationId = null
                      , Guid? provinceId = null, Guid? districId = null, Guid? siteId = null);
        ResultModel Create(IndicatorImportModel aggregatedValue);
        ResultModel Update(IndicatorImportModel aggregatedValue);
        ResultModel Delete(Guid id);
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
        ResultModel PopulateData(string indicator, int year, int month, int? day = null, bool all = false, bool makeDeletion = false);
        ResultModel GetChartData(string indicator, int year, int quater, string provinceCode, string districtCode, int? month = null, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null);
        ResultModel GetIndicatorValues(string provinceCode, string districtCode, string indicatorGroup, string indicatorCode, int year
            , int? quarter = null, int? month = null, string ageGroups = "", string keyPopulations = "", string genders = "", string sites = "");

        ResultModel ImportExcel(IFormFile file, string username = null);
        ResultModel ImportIndicator(List<IndicatorImportModel> importValues, List<object> errorRow = null, string username = null);
        ResultModel ImportIndicator(AggregatedData aggregatedData, string username = null);
        ResultModel ClearAll();
    }

    public class AggregatedValueService : IAggregatedValueService
    {
        private const string INDICATOR_DENOMINATOR = nameof(IndicatorElasticModel.Denominator);
        private const string INDICATOR_NUMERATOR = nameof(IndicatorElasticModel.Numerator);


        private readonly AppDBContext _dBContext;
        private readonly ElasticClient _elasticClient;

        private IConfiguration Configuration { get; }
        private string _aggregatedValueIndex { get; set; }

        public AggregatedValueService(AppDBContext dBContext, ElasticClient elasticClient, IConfiguration configuration)
        {
            _dBContext = dBContext;
            _elasticClient = elasticClient;
            Configuration = configuration;
            _aggregatedValueIndex = Configuration["elasticsearch:index"];
        }

        public ResultModel GetAggregatedValues(int year, int? quarter, int? month, string indicatorGroup, string indicatorCode, string groupBy, string provinceCode, string districtCode)
        {
            var result = new ResultModel();
            result.Succeed = true;
            try
            {
                var dims = groupBy == "AgeGroup" ? _dBContext.AgeGroups.ToList().Select(s => s.Adapt<DimensionViewModel>())
                    : groupBy == "KeyPopulation" ? _dBContext.KeyPopulations.ToList().Select(s => s.Adapt<DimensionViewModel>())
                    : groupBy == "Gender" ? _dBContext.Gender.ToList().Select(s => s.Adapt<DimensionViewModel>())
                    : _dBContext.Sites.ToList().Select(s => s.Adapt<DimensionViewModel>());
                var periodType = month == null ? "Quarter" : "Month";
                var _districts = !string.IsNullOrEmpty(districtCode) ? districtCode.Split(',').ToList() : new List<string>();
                var response = SearchOnElastic(provinceCode: provinceCode, districts: _districts
                    , indicatorGroup: indicatorGroup, indicatorCode: indicatorCode
                    , year: year, quarter: quarter, month: month, groupBy: groupBy, onlyTotal: true, periodType: periodType);
                var data = response.Aggregations.Terms(groupBy).Buckets.Select(s => new IndicatorModel
                {
                    Name = s.Key,
                    Value = new IndicatorValue
                    {
                        Value = (int)s.Sum(INDICATOR_NUMERATOR).Value,
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
            var aggregatedValues = _dBContext.AggregatedValues.Where(w => (w.Month + w.Year * 100 <= limit) && sites.Contains(w.SiteId) && w.Indicator.Name == indicator);
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
                aggregatedValues = aggregatedValues.Where(s => _genders.Contains(s.GenderId));
            }
            if (!string.IsNullOrEmpty(clinnics))
            {
                var _clinnics = clinnics.Split(',').Select(s => Guid.Parse(s));
                aggregatedValues = aggregatedValues.Where(s => _clinnics.Contains(s.SiteId));
            }
            var groupIndicator = aggregatedValues.ToList().GroupBy(g => g.Month);
            var data = groupIndicator.Select(s => new IndicatorModel
            {
                Order = s.FirstOrDefault().Month.Value + s.FirstOrDefault().Year * 100,
                Name = string.Format("{0}-{1}", s.FirstOrDefault().Month, s.FirstOrDefault().Year),
                Value = new IndicatorValue
                {
                    Value = s.Sum(_ => _.Numerator),
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
                return (new QueryContainerDescriptor<IndicatorElasticModel>()).Terms(t => t.Field(f => f.Site.Suffix("keyword")).Terms(sites));
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

        private Dictionary<string, IAggregationContainer> BuildAggregationQuery(string groupBy)
        {
            var aggregations = new Dictionary<string, IAggregationContainer>();
            var sumAggregations = new Dictionary<string, IAggregationContainer>();
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
            , int year, int? quarter = null, int? month = null
            , List<string> ageGroups = null, List<string> keyPopulations = null, List<string> genders = null, List<string> sites = null
            , string groupBy = "", bool onlyTotal = false, string periodType = "Month")
        {

            var queryContainers = new List<QueryContainer>();
            var filterContainers = new List<QueryContainer>();
            var shouldContainers = new List<QueryContainer>();
            queryContainers.Add((new QueryContainerDescriptor<IndicatorElasticModel>()).Match(m => m.Field(f => f.PeriodType).Query(periodType)));
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
                queryContainers.Add((new QueryContainerDescriptor<IndicatorElasticModel>()).Terms(t => t.Field(f => f.AgeGroup.Suffix("keyword")).Terms(ageGroups)));
            }
            if (keyPopulations != null && keyPopulations.Count > 0)
            {
                queryContainers.Add((new QueryContainerDescriptor<IndicatorElasticModel>()).Terms(t => t.Field(f => f.KeyPopulation.Suffix("keyword")).Terms(keyPopulations)));
            }
            if (genders != null && genders.Count > 0)
            {
                queryContainers.Add((new QueryContainerDescriptor<IndicatorElasticModel>()).Terms(t => t.Field(f => f.KeyPopulation.Suffix("keyword")).Terms(genders)));
            }
            if (onlyTotal)
            {
                queryContainers.Add((new QueryContainerDescriptor<IndicatorElasticModel>()).Match(m => m.Field(f => f.IsTotal).Query("true")));
            }
            var searchRequest = new SearchRequest<IndicatorElasticModel>(_aggregatedValueIndex)
            {
                From = 0,
                Size = 1,
                Query = new BoolQuery { Must = queryContainers, Filter = filterContainers, Should = shouldContainers },
                Aggregations = BuildAggregationQuery(groupBy),
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
            var periodType = month == null ? "Quarter" : "Month";
            //old data
            var preYear = quarter != null ? (quarter > 1 ? year : year - 1) : month != null ? (month > 1 ? year : year - 1) : year - 1;
            int? preMonth = month != null ? (month > 1 ? month - 1 : 12) : null;
            int? preQuarter = quarter != null ? (quarter > 1 ? quarter - 1 : 4) : null;
            var response2 = SearchOnElastic(provinceCode: provinceCode, districts: _districts, indicatorGroup: indicatorGroup, indicatorCode: indicatorCode
                , year: preYear, quarter: preQuarter, month: preMonth
                , ageGroups: _ageGroups, keyPopulations: _keyPopulations, genders: _genders, sites: _sites
                , groupBy: "IndicatorCode", periodType: periodType);
            var predata = response2.Aggregations.Terms("IndicatorCode").Buckets.Select(s => new IndicatorModel
            {
                Name = s.Key,
                Value = new IndicatorValue
                {
                    Value = (int)s.Sum(INDICATOR_NUMERATOR).Value,
                    Numerator = (int)s.Sum(INDICATOR_NUMERATOR).Value,
                    Denominator = (int)s.Sum(INDICATOR_DENOMINATOR).Value,
                }
            }).ToList();
            predata.ForEach(s =>
            {
                var indicator = indicators.FirstOrDefault(f => f.Code == s.Name);
                if (indicator != null)
                {
                    s.Name = indicator.Name;
                    s.Group = indicator.IndicatorGroup.Name;
                    s.Order = indicator.Order;
                    s.Value.DataType = indicator.DataType;
                    if (indicator.DataType == DataType.Percent)
                    {
                        var deIndicator = indicators.FirstOrDefault(f => f.Id == indicator.DenominatorIndicatorId);
                        var deData = predata.FirstOrDefault(f => f.Name == deIndicator.Name || f.Name == deIndicator.Code);
                        if (deData != null)
                        {
                            s.Value.Denominator = deData.Value.Numerator;
                        }
                    }
                }
            });

            //new data
            var response = SearchOnElastic(provinceCode: provinceCode, districts: _districts, indicatorGroup: indicatorGroup, indicatorCode: indicatorCode
                , year: year, quarter: quarter, month: month
                , ageGroups: _ageGroups, keyPopulations: _keyPopulations, genders: _genders, sites: _sites
                , groupBy: "IndicatorCode", periodType: periodType);
            var data = response.Aggregations.Terms("IndicatorCode").Buckets.Select(s => new IndicatorModel
            {
                Name = s.Key,
                Value = new IndicatorValue
                {
                    Value = (int)s.Sum(INDICATOR_NUMERATOR).Value,
                    Numerator = (int)s.Sum(INDICATOR_NUMERATOR).Value,
                    Denominator = (int)s.Sum(INDICATOR_DENOMINATOR).Value,
                }
            }).ToList();
            data.ForEach(s =>
            {
                var indicator = indicators.FirstOrDefault(f => f.Code == s.Name || f.Name == s.Name);

                if (indicator != null)
                {
                    s.Name = indicator.Name;
                    s.Group = indicator.IndicatorGroup.Name;
                    s.Order = indicator.Order;
                    s.Value.DataType = indicator.DataType;
                    if (indicator.DataType == DataType.Percent)
                    {
                        var deIndicator = indicators.FirstOrDefault(f => f.Id == indicator.DenominatorIndicatorId);
                        var deData = data.FirstOrDefault(f => f.Name == deIndicator.Name || f.Name == deIndicator.Code);
                        if (deData != null)
                        {
                            s.Value.Denominator = deData.Value.Numerator;
                        }
                    }
                    var pre = predata.FirstOrDefault(f => f.Name == s.Name);
                    if (pre != null)
                    {
                        var p = s.Value.DataType == DataType.Number
                                                            ? (double)(s.Value.Numerator - pre.Value.Numerator) / pre.Value.Numerator
                                                            : (double)(s.Value.Numerator * pre.Value.Denominator
                                                                        - pre.Value.Numerator * s.Value.Denominator)
                                                                        / (double)(s.Value.Denominator * pre.Value.Denominator);
                        s.Trend = new IndicatorTrend
                        {
                            ComparePercent = p.Value >= 0 ? p.Value : -p.Value,
                            Direction = p >= 0 ? 1 : -1,
                        };
                    }
                }
            });
            return new ResultModel()
            {
                Data = data.OrderBy(s => s.Order).ToList(),
                Succeed = true,
            };
        }

        public ResultModel PopulateData(string indicator, int year, int month, int? day = null, bool all = false, bool makeDeletion = false)
        {
            if (makeDeletion)
            {
                _elasticClient.DeleteByQuery<IndicatorElasticModel>(del => del.Index(_aggregatedValueIndex).Query(q => q.QueryString(qs => qs.Query("*"))));
            }

            var data = _dBContext.AggregatedValues
                                .Where(w => all || (w.Month == month && w.Year == year && w.Indicator.Name == indicator))
                                .Select(s => new IndicatorElasticModel
                                {
                                    IndicatorName = s.Indicator.Name,
                                    IndicatorCode = s.Indicator.Code,
                                    IndicatorGroup = s.Indicator.IndicatorGroup.Name,
                                    IsTotal = s.Indicator.IsTotal.Value,
                                    Quarter = s.Quarter,
                                    DistrictCode = s.Site.District.Code,
                                    ProvinceCode = s.Site.District.Province.Code,
                                    Location = (s.Site.Lat != null && s.Site.Lng != null) ? new GeoCoordinate(s.Site.Lat.Value, s.Site.Lng.Value) : null,
                                    ValueType = s.DataType == DataType.Number ? 1 : 2,
                                    Denominator = s.Denominator,
                                    Numerator = s.Numerator,
                                    AgeGroup = s.AgeGroup.Name,
                                    KeyPopulation = s.KeyPopulation.Name,
                                    Site = s.Site.Name,
                                    Gender = s.Gender.Name,
                                    PeriodType = s.PeriodType,

                                    Month = !all ? month : s.Month,
                                    Year = !all ? year : s.Year,
                                    Day = !all ? day : s.Day,
                                }).ToList();
            foreach (var s in data)
            {
                if (s.PeriodType.ToLower() == "month")
                {
                    s.Date = new DateTime(s.Year, s.Month.Value, DateTime.DaysInMonth(s.Year, s.Month.Value));
                }
                else if (s.PeriodType.ToLower() == "quarter")
                {
                    var _day = s.Quarter.Value == 1 ? 31 : s.Quarter.Value == 2 ? 30 : s.Quarter.Value == 3 ? 30 : 31;
                    var _month = s.Quarter.Value == 1 ? 3 : s.Quarter.Value == 2 ? 6 : s.Quarter.Value == 3 ? 9 : 12;
                    s.Date = new DateTime(s.Year, _month, _day);
                }
            }
            var pageSize = 100;
            var pageCount = data.Count / pageSize + (data.Count % pageSize != 0 ? 1 : 0);
            for (var index = 0; index < pageCount; index++)
            {
                var response = _elasticClient.IndexMany(data.Skip(pageSize * index).Take(pageSize), _aggregatedValueIndex);
            }

            return new ResultModel()
            {
                Succeed = true,
            };
        }

        private IndicatorImportModel VerifyTime(IndicatorImportModel data, out string error)
        {
            error = "";
            if (data.PeriodType.Equals("Year", StringComparison.OrdinalIgnoreCase))
            {
                data.Quarter = null;
                data.Month = null;
                data.Day = null;
            } //no need quarter, month and day when period is year
            else if (data.PeriodType.Equals("Quarter", StringComparison.OrdinalIgnoreCase))
            {
                data.Month = null;
                data.Day = null;
            } //no need month and day when period is quarter
            else if (data.PeriodType.Equals("Month", StringComparison.OrdinalIgnoreCase))
            {
                data.Day = null;
            } //no need day when period is month
            var hasQuarter = data.Quarter != null;
            if ((data.PeriodType.Equals("Quarter", StringComparison.OrdinalIgnoreCase)
                || data.PeriodType.Equals("Month", StringComparison.OrdinalIgnoreCase)
                || data.PeriodType.Equals("Day", StringComparison.OrdinalIgnoreCase)) && !hasQuarter)
            {
                error = "No quarter";
            } //check quarter is not null
            var hasMonth = data.Month != null && hasQuarter;
            if ((data.PeriodType.Equals("Month", StringComparison.OrdinalIgnoreCase)
                || data.PeriodType.Equals("Day", StringComparison.OrdinalIgnoreCase)) && !hasMonth)
            {
                error = "No month";
            } //check month is not null
            var hasDay = data.Day != null && hasMonth;
            if ((data.PeriodType.Equals("Day", StringComparison.OrdinalIgnoreCase)) && !hasDay)
            {
                error = "No day";
            } //check day is not null
            return data;
        }

        private Guid? FindCategoryId(string category, string name, out UndefinedDimValue undefinedDimValue)
        {
            undefinedDimValue = null;
            var typeOfCategory = category == "AgeGroup" ? typeof(AgeGroup)
                               : category == "KeyPopulation" ? typeof(KeyPopulation)
                               : category == "Gender" ? typeof(Gender)
                               : category == "Site" ? typeof(Site)
                               : null;
            if (typeOfCategory == null)
            {
                return null;
            }
            var dim = _dBContext.Set(typeOfCategory).Adapt<List<Dimension>>()
                .FirstOrDefault(s => !s.IsDeleted && (s.Name == name || s.Code == name));
            if (dim != null)
            {
                return dim.Id;
            }
            else
            {
                var na = _dBContext.Set(typeOfCategory).Adapt<List<Dimension>>().FirstOrDefault(s => s.Name == "N/A");
                undefinedDimValue = new UndefinedDimValue
                {
                    Dimension = category,
                    UndefinedValue = name,
                };
                if (na != null)
                {
                    return na.Id;
                }
                else
                {
                    return null;
                }
            }
        }

        private Dictionary<string, Guid> GetDimensionValues(IndicatorImportModel data
            , int index
            , out Dictionary<string, UndefinedDimValue> localUndefinedDimValues
            , out List<object> errors)
        {
            var _localUndefinedDimValues = new Dictionary<string, UndefinedDimValue>();
            var _errors = new List<object>();
            var dim = new Dictionary<string, string>()
                    {
                        { "AgeGroup", data.AgeGroup },
                        { "Gender", data.Gender },
                        { "KeyPopulation", data.KeyPopulation },
                        { "Site", data.Site },
                    };
            var definedDimValue = new Dictionary<string, Guid>();
            dim.ToList().ForEach(e =>
            {
                UndefinedDimValue undefinedDimValue;
                var categoryId = FindCategoryId(e.Key, e.Value, out undefinedDimValue);
                if (categoryId != null)
                {
                    definedDimValue.Add(e.Key, categoryId.Value);
                }
                else
                {
                    _errors.Add(new
                    {
                        Row = index,
                        Error = string.Format("No {0} data", e.Key)
                    });
                }
                if (undefinedDimValue != null)
                {
                    var key = string.Format("{0}:{1}", e.Key, e.Value);
                    _localUndefinedDimValues.TryAdd(key, undefinedDimValue);
                }
            });
            localUndefinedDimValues = _localUndefinedDimValues;
            errors = _errors;
            return definedDimValue;
        }

        public ResultModel ImportIndicator(List<IndicatorImportModel> importValues, List<object> errorRows = null, string username = null)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new ResultModel
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = "Not found user"
                    }
                };
            }

            try
            {
                errorRows = errorRows != null ? errorRows : new List<object>();
                int succeed = 0;
                int succeedWithUndefinedDimValue = 0;
                int updated = 0;
                var undefinedDimValues = new List<UndefinedDimValue>();
                var permissions = _dBContext.DataPermissions
                    .Where(s => s.Type == DataPermissionType.Write && s.Username == username)
                    .ToList();
                var percentValues = new List<AggregatedValue>();
                for (int i = 0; i < importValues.Count; i++)
                {
                    var isValid = true;
                    var invalidMessage = "";
                    var data = importValues[i];
                    var _index = data.RowIndex != null ? data.RowIndex.Value + 1 : i + 1;
                    #region check time
                    string error;
                    data = VerifyTime(data, out error);
                    if (error.Length > 0)
                    {
                        errorRows.Add(new
                        {
                            Row = _index,
                            Error = error,
                        });
                        continue;
                    }
                    #endregion
                    #region map category alias name and add unsolved dimension value
                    var localUndefinedDimValues = new Dictionary<string, UndefinedDimValue>();
                    var errors = new List<object>();
                    var definedDimValue = GetDimensionValues(data, _index, out localUndefinedDimValues, out errors);
                    errorRows.AddRange(errors);
                    if (errors.Count > 0)
                    {
                        continue;
                    }
                    var indicator = _dBContext.Indicators.FirstOrDefault(f => !f.IsDeleted && (f.Name.Equals(data.Indicator) || f.Code.Equals(data.Indicator)));
                    if (indicator == null)
                    {
                        errorRows.Add(new
                        {
                            Row = _index,
                            Error = "Can not find indicator",
                        });
                        continue;
                    }
                    #endregion
                    Guid ageGroupId; definedDimValue.TryGetValue("AgeGroup", out ageGroupId);
                    Guid siteId; definedDimValue.TryGetValue("Site", out siteId);
                    Guid keyPopulationId; definedDimValue.TryGetValue("KeyPopulation", out keyPopulationId);
                    Guid genderId; definedDimValue.TryGetValue("Gender", out genderId);
                    #region check permission
                    if (username != "admin")
                    {
                        var site = _dBContext.Sites.Include(s => s.District).FirstOrDefault(s => s.Id == siteId);
                        if (site == null || site.Name == "N/A")
                        {
                            errorRows.Add(new
                            {
                                Row = _index,
                                Error = "Undefined Site",
                            });
                            continue;
                        }
                        var p = permissions.FirstOrDefault(s => s.IndicatorId == indicator.Id && s.ProvinceId == site.District.ProvinceId);
                        if (p == null)
                        {
                            errorRows.Add(new
                            {
                                Row = _index,
                                Error = "User doesn't have permission to insert or update",
                            });
                            continue;
                        }
                    }
                    #endregion
                    #region add aggregated value to database
                    var current = _dBContext.AggregatedValues.FirstOrDefault(f => f.SiteId == siteId
                                                                               && f.KeyPopulationId == keyPopulationId
                                                                               && f.AgeGroupId == ageGroupId
                                                                               && f.GenderId == genderId
                                                                               && f.IndicatorId == indicator.Id
                                                                               && f.Day == data.Day
                                                                               && f.Month == data.Month
                                                                               && f.Quarter == data.Quarter
                                                                               && f.Year == data.Year
                                                                               && f.PeriodType == data.PeriodType);
                    if (current != null)
                    {
                        if (current.UnsolvedDimValues != null && current.UnsolvedDimValues.Count == 0 && localUndefinedDimValues.Count == 0)
                        {
                            current.Numerator = data.Numerator;
                            current.Denominator = data.Denominator != null ? data.Denominator.Value : 0;
                            _dBContext.AggregatedValues.Update(current);
                            updated++;
                            if (current.DataType == DataType.Percent)
                            {
                                percentValues.Add(current);
                            }
                        }
                    }
                    else
                    {
                        var aggregatedValue = new AggregatedValue
                        {
                            Id = Guid.NewGuid(),
                            AgeGroupId = ageGroupId,
                            GenderId = genderId,
                            KeyPopulationId = keyPopulationId,
                            SiteId = siteId,
                            Numerator = data.Numerator,
                            Denominator = data.Denominator != null ? data.Denominator.Value : 0,
                            DataType = data.ValueType == 1 ? Data.Enums.DataType.Number : Data.Enums.DataType.Percent,
                            CreatedBy = "",
                            IsDeleted = false,
                            DateCreated = DateTime.Now,
                            IndicatorId = indicator.Id,
                            Day = data.Day,
                            Month = data.Month,
                            Quarter = data.Quarter,
                            Year = data.Year,
                            PeriodType = data.PeriodType,
                            IsValid = isValid,
                            InvalidMessage = invalidMessage,
                        };
                        _dBContext.AggregatedValues.Add(aggregatedValue);
                        if (localUndefinedDimValues.Count > 0)
                        {
                            succeedWithUndefinedDimValue++;
                            localUndefinedDimValues.ToList().ForEach(u =>
                            {
                                var undefinedDimValue = _dBContext.UndefinedDimValues.FirstOrDefault(s => s.Dimension == u.Value.Dimension
                                                                            && s.UndefinedValue == u.Value.UndefinedValue);
                                if (undefinedDimValue == null)
                                {
                                    undefinedDimValue = u.Value;
                                    _dBContext.UndefinedDimValues.Add(undefinedDimValue);
                                }
                                _dBContext.UnsolvedDimValues.Add(new UnsolvedDimValue()
                                {
                                    AggregatedValueId = aggregatedValue.Id,
                                    UndefinedDimValueId = undefinedDimValue.Id,
                                });
                            });
                            undefinedDimValues = undefinedDimValues.Union(localUndefinedDimValues.Select(u => u.Value).ToList()).ToList();
                        }
                        else
                        {
                            if (aggregatedValue.DataType == DataType.Percent)
                            {
                                percentValues.Add(aggregatedValue);
                            }
                            succeed++;
                        }
                    }
                    #endregion
                }
                _dBContext.SaveChanges();
                percentValues.ForEach(value =>
                {
                    var deIndicator = _dBContext.Indicators.FirstOrDefault(s => !s.IsDeleted && s.Id == value.Indicator.DenominatorIndicatorId);
                    if (deIndicator != null)
                    {
                        var denominator = _dBContext.AggregatedValues.FirstOrDefault(f => f.SiteId == value.SiteId
                                                                               && f.KeyPopulationId == value.KeyPopulationId
                                                                               && f.AgeGroupId == value.AgeGroupId
                                                                               && f.GenderId == value.GenderId
                                                                               && f.IndicatorId == deIndicator.Id
                                                                               && (value.PeriodType.ToLower() != "day" || f.Day == value.Day)
                                                                               && ((value.PeriodType.ToLower() != "day" && (value.PeriodType.ToLower() != "month")) || f.Month == value.Month)
                                                                               && ((value.PeriodType.ToLower() != "day" && (value.PeriodType.ToLower() != "month") && (value.PeriodType.ToLower() != "quarter")) || f.Quarter == value.Quarter)
                                                                               && f.Year == value.Year
                                                                               && f.PeriodType == value.PeriodType);
                        if (denominator != null)
                        {
                            value.Denominator = denominator.Numerator;
                            if (value.Numerator > value.Denominator)
                            {
                                value.IsValid = false;
                                value.InvalidMessage = "Numerator > Denominator";
                            }
                        }
                        else
                        {
                            value.IsValid = false;
                            value.InvalidMessage = "Can not find the denominator data";
                        }
                    }
                    else
                    {
                        value.IsValid = false;
                        value.InvalidMessage = "Can not find the denominator data";
                    }
                    _dBContext.AggregatedValues.Update(value);
                });
                _dBContext.SaveChanges();
                return new ResultModel()
                {
                    Succeed = true,
                    Error = null,
                    Data = new
                    {
                        Succeed = succeed,
                        SucceedWithUndefinedDimValue = succeedWithUndefinedDimValue,
                        Updated = updated,
                        ErrorRows = errorRows,
                        UndefinedDimValues = undefinedDimValues.Select(s => new
                        {
                            s.Dimension,
                            s.UndefinedValue,
                        })
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResultModel()
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = ex.Message
                    },
                };
            }
        }

        public ResultModel ImportExcel(IFormFile file, string username = null)
        {
            var importedValues = new List<IndicatorImportModel>();
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                stream.Position = 0;
                XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                ISheet sheet;
                sheet = xssWorkbook.GetSheetAt(0);
                for (var i = 1; i <= sheet.LastRowNum; i++)
                {
                    var row = sheet.GetRow(i);
                    var period = row.GetCell(0).ToString();
                    var year = int.Parse(row.GetCell(1).ToString());
                    var quarter = TryParse(row.GetCell(2).ToString());
                    var month = TryParse(row.GetCell(3) != null ? row.GetCell(3).ToString() : null);
                    var day = TryParse(row.GetCell(4) != null ? row.GetCell(4).ToString() : null);
                    var indicator = row.GetCell(5).ToString();
                    var gender = row.GetCell(6).ToString();
                    var ageGroup = row.GetCell(7).ToString();
                    var keyPopulation = row.GetCell(8).ToString();
                    var site = row.GetCell(9).ToString();
                    var valueType = row.GetCell(10) != null && row.GetCell(10).ToString().Length > 0 ? int.Parse(row.GetCell(10).ToString()) : 0;
                    var numerator = row.GetCell(11) != null && row.GetCell(11).ToString().Length > 0 ? int.Parse(row.GetCell(11).ToString()) : 0;
                    var denominator = row.GetCell(12) != null && row.GetCell(12).ToString().Length > 0 ? int.Parse(row.GetCell(12).ToString()) : 0;
                    importedValues.Add(new IndicatorImportModel
                    {
                        PeriodType = period,
                        Year = year,
                        Quarter = quarter,
                        Month = month,
                        Day = day,
                        Indicator = indicator,
                        Gender = gender,
                        AgeGroup = ageGroup,
                        KeyPopulation = keyPopulation,
                        Site = site,
                        ValueType = valueType,
                        Numerator = numerator,
                        Denominator = denominator,
                    });
                }
            }

            return ImportIndicator(importedValues, username: username);
        }

        private int? TryParse(string value)
        {
            int? result = null;
            try
            {
                result = int.Parse(value);
            }
            catch (Exception) { }
            return result;
        }

        public ResultModel Get(int? pageIndex = 0, int? pageSize = int.MaxValue
            , string period = null, int? year = null, int? quarter = null, int? month = null
            , Guid? indicatorId = null, Guid? ageGroupId = null, Guid? genderId = null, Guid? keyPopulationId = null
            , Guid? provinceId = null, Guid? districId = null, Guid? siteId = null)
        {
            var filter = _dBContext.AggregatedValues.Where(w => true);
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
                var sites = _dBContext.Sites.Where(s => s.DistrictId == districId).Select(s => s.Id);
                filter = filter.Where(w => sites.Contains(w.SiteId));
            }
            else if (provinceId != null)
            {
                var districs = _dBContext.Districts.Where(s => s.ProvinceId == provinceId).Select(s => s.Id);
                var sites = _dBContext.Sites.Where(s => districs.Contains(s.DistrictId)).Select(s => s.Id);
                filter = filter.Where(w => sites.Contains(w.SiteId));
            }
            return new PagingModel
            {
                Succeed = true,
                Total = filter.Count(),
                PageCount = filter.PageCount(pageSize.Value),
                Data = filter.Skip((int)(pageIndex * pageSize)).Take((int)pageSize).Adapt<IEnumerable<AggregatedValueViewModel>>(),
            };
        }

        public ResultModel Create(IndicatorImportModel aggregatedValue)
        {
            var rs = new ResultModel();
            #region check time
            string error;
            aggregatedValue = VerifyTime(aggregatedValue, out error);
            if (error.Length > 0)
            {
                return new ResultModel
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = error
                    },
                };
            }
            #endregion
            #region map category alias name and add unsolved dimension value
            var localUndefinedDimValues = new Dictionary<string, UndefinedDimValue>();
            var errors = new List<object>();
            var definedDimValue = GetDimensionValues(aggregatedValue, 0, out localUndefinedDimValues, out errors);
            if (errors.Count > 0)
            {
                return new ResultModel
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = JsonConvert.SerializeObject(errors)
                    },
                };
            }
            if (localUndefinedDimValues.Count > 0)
            {
                return new ResultModel
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = JsonConvert.SerializeObject(localUndefinedDimValues.Select(s => string.Format("Category {0} does not have value: {1}", s.Key, s.Value.UndefinedValue))),
                    }
                };
            }
            #endregion
            #region add aggregated value to database
            var indicator = _dBContext.Indicators.FirstOrDefault(f => f.Name.Equals(aggregatedValue.Indicator));
            Guid ageGroupId; definedDimValue.TryGetValue("AgeGroup", out ageGroupId);
            Guid siteId; definedDimValue.TryGetValue("Site", out siteId);
            Guid keyPopulationId; definedDimValue.TryGetValue("KeyPopulation", out keyPopulationId);
            Guid genderId; definedDimValue.TryGetValue("Gender", out genderId);
            var current = _dBContext.AggregatedValues.FirstOrDefault(f => f.SiteId == siteId
                                                                       && f.KeyPopulationId == keyPopulationId
                                                                       && f.AgeGroupId == ageGroupId
                                                                       && f.GenderId == genderId
                                                                       && f.IndicatorId == indicator.Id
                                                                       && f.Day == aggregatedValue.Day
                                                                       && f.Month == aggregatedValue.Month
                                                                       && f.Quarter == aggregatedValue.Quarter
                                                                       && f.Year == aggregatedValue.Year
                                                                       && f.PeriodType == aggregatedValue.PeriodType);
            if (current != null)
            {
                return new ResultModel
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = "existed aggregated value",
                    }
                };
            }
            else
            {
                var _aggregatedValue = new AggregatedValue
                {
                    Id = Guid.NewGuid(),
                    AgeGroupId = ageGroupId,
                    GenderId = genderId,
                    KeyPopulationId = keyPopulationId,
                    SiteId = siteId,
                    Numerator = aggregatedValue.Numerator,
                    Denominator = aggregatedValue.Denominator.Value,
                    DataType = aggregatedValue.ValueType == 1 ? Data.Enums.DataType.Number : Data.Enums.DataType.Percent,
                    CreatedBy = "",
                    IsDeleted = false,
                    DateCreated = DateTime.Now,
                    IndicatorId = indicator.Id,
                    Day = aggregatedValue.Day,
                    Month = aggregatedValue.Month,
                    Quarter = aggregatedValue.Quarter,
                    Year = aggregatedValue.Year,
                    PeriodType = aggregatedValue.PeriodType,
                };
                _dBContext.AggregatedValues.Add(_aggregatedValue);
            }
            #endregion
            rs.Succeed = _dBContext.SaveChanges() > 0;
            return rs;
        }

        public ResultModel Delete(Guid id)
        {
            var aggregatedValue = _dBContext.AggregatedValues.FirstOrDefault(s => s.Id == id);
            if (aggregatedValue == null)
            {
                return new ResultModel
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = "No existed"
                    },
                };
            }
            foreach (var u in aggregatedValue.UnsolvedDimValues)
            {
                _dBContext.UnsolvedDimValues.Remove(u);
            }
            _dBContext.AggregatedValues.Remove(aggregatedValue);
            var rs = new ResultModel()
            {
                Succeed = _dBContext.SaveChanges() > 0
            };
            return rs;
        }

        public ResultModel Update(IndicatorImportModel aggregatedValue)
        {
            var rs = new ResultModel();
            #region check time
            string error;
            aggregatedValue = VerifyTime(aggregatedValue, out error);
            if (error.Length > 0)
            {
                return new ResultModel
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = error,
                    }
                };
            }
            #endregion
            #region map category alias name and add unsolved dimension value
            var localUndefinedDimValues = new Dictionary<string, UndefinedDimValue>();
            var errors = new List<object>();
            var definedDimValue = GetDimensionValues(aggregatedValue, 0, out localUndefinedDimValues, out errors);
            if (errors.Count > 0)
            {
                return new ResultModel
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = JsonConvert.SerializeObject(errors),
                    }
                };
            }
            if (localUndefinedDimValues.Count > 0)
            {
                return new ResultModel
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = JsonConvert.SerializeObject(localUndefinedDimValues.Select(s => string.Format("Category {0} does not have value: {1}", s.Key, s.Value.UndefinedValue))),
                    }
                };
            }
            #endregion
            #region add aggregated value to database
            var indicator = _dBContext.Indicators.FirstOrDefault(f => f.Name.Equals(aggregatedValue.Indicator));
            Guid ageGroupId; definedDimValue.TryGetValue("AgeGroup", out ageGroupId);
            Guid siteId; definedDimValue.TryGetValue("Site", out siteId);
            Guid keyPopulationId; definedDimValue.TryGetValue("KeyPopulation", out keyPopulationId);
            Guid genderId; definedDimValue.TryGetValue("Gender", out genderId);
            var current = _dBContext.AggregatedValues.FirstOrDefault(f => f.SiteId == siteId
                                                                       && f.KeyPopulationId == keyPopulationId
                                                                       && f.AgeGroupId == ageGroupId
                                                                       && f.GenderId == genderId
                                                                       && f.IndicatorId == indicator.Id
                                                                       && f.Day == aggregatedValue.Day
                                                                       && f.Month == aggregatedValue.Month
                                                                       && f.Quarter == aggregatedValue.Quarter
                                                                       && f.Year == aggregatedValue.Year
                                                                       && f.PeriodType == aggregatedValue.PeriodType);
            if (current == null)
            {
                return new ResultModel
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = "no existed aggregated value",
                    }
                };
            }
            else
            {
                current.Numerator = aggregatedValue.Numerator;
                current.Denominator = aggregatedValue.Denominator.Value;
                _dBContext.AggregatedValues.Update(current);
            }
            #endregion
            rs.Succeed = _dBContext.SaveChanges() > 0;
            return rs;
        }

        private int GetQuarter(int month)
        {
            return month < 4 ? 1 : month < 7 ? 2 : month < 10 ? 3 : 4;
        }

        public ResultModel ImportIndicator(AggregatedData aggregatedData, string username)
        {
            var rs = new ResultModel();
            var importData = new List<IndicatorImportModel>();
            var _indicators = _dBContext.Indicators;
            var _errorRows = new List<object>();
            int month, year;
            if (!int.TryParse(aggregatedData.year, out year))
            {
                return new ResultModel()
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = "Year is not defined"
                    }
                };
            }
            if (!int.TryParse(aggregatedData.month, out month))
            {
                return new ResultModel()
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = "Month is not defined"
                    }
                };
            }
            for (int i = 0; i < aggregatedData.datas.Count; i++)
            {
                var row = aggregatedData.datas[i];
                var indicator = _indicators.FirstOrDefault(s => s.Code == row.indicator_code || s.Name == row.indicator_code);
                if (indicator == null)
                {
                    _errorRows.Add(new
                    {
                        Row = i + 1,
                        Error = string.Format("No {0} indicator data", row.indicator_code)
                    });
                    continue;
                }
                if (!int.TryParse(row.data.value, out int _value))
                {
                    _errorRows.Add(new
                    {
                        Row = i + 1,
                        Error = string.Format("Value is not defined")
                    });
                    continue;
                }
                var data = new IndicatorImportModel()
                {
                    RowIndex = i,
                    AgeGroup = row.data.age_group,
                    KeyPopulation = row.data.key_population,
                    Gender = row.data.sex,
                    Site = row.site_code,
                    Numerator = int.Parse(row.data.value),
                    PeriodType = row.data.type,
                    Year = year,
                    Quarter = GetQuarter(month),
                    Day = null,
                    ValueType = indicator.DataType == DataType.Number ? 1 : 2,
                    Indicator = indicator.Name,
                    Denominator = row.data.denominatorValue,
                    Month = null,
                };
                if (row.data.type == "month")
                {
                    data.Month = month;
                }
                importData.Add(data);
            };
            return ImportIndicator(importData, _errorRows, username);
        }

        public ResultModel ClearAll()
        {
            using var transaction = _dBContext.Database.BeginTransaction();
            try
            {
                _elasticClient.DeleteByQuery<IndicatorElasticModel>(del => del.Index(_aggregatedValueIndex).Query(q => q.QueryString(qs => qs.Query("*"))));
                _dBContext.UnsolvedDimValues.RemoveRange(_dBContext.UnsolvedDimValues);
                _dBContext.UndefinedDimValues.RemoveRange(_dBContext.UndefinedDimValues);
                _dBContext.AggregatedValues.RemoveRange(_dBContext.AggregatedValues);
                _dBContext.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                return new ResultModel()
                {
                    Succeed = false,
                };
            }
            return new ResultModel()
            {
                Succeed = true,
            };
        }
    }
}
