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
using System.Threading.Tasks;
using System.Net.Http;

namespace PQM_WebApp.Service
{
    public interface IAggregatedValueService
    {
        ResultModel Get(int? pageIndex = 0, int? pageSize = int.MaxValue
                      , string period = null, int? year = null, int? quarter = null, int? month = null
                      , Guid? indicatorId = null, Guid? ageGroupId = null, Guid? genderId = null, Guid? keyPopulationId = null
                      , Guid? provinceId = null, Guid? districId = null, Guid? siteId = null, Guid? indicatorGroupId = null);
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
        ResultModel GetAggregatedValues(int year, int? quarter, int? month, string indicatorGroup, string indicatorCode, string groupBy, string provinceCode, string districtCode, Guid? indicatorGroupId = null);
        ResultModel PopulateData(string indicator, int year, int month, int? day = null, bool all = false, bool makeDeletion = false);
        ResultModel GetChartData(string indicator, int year, int quater, string provinceCode, string districtCode, int? month = null, string ageGroups = null, string keyPopulations = null, string genders = null, string clinnics = null);
        ResultModel GetIndicatorValues(string provinceCode, string districtCode, string indicatorGroup, string indicatorCode, int year
            , int? quarter = null, int? month = null, string ageGroups = "", string keyPopulations = "", string genders = "", string sites = "", Guid? indicatorGroupId = null);

        ResultModel ImportExcel(IFormFile file, string username = null);
        ResultModel ImportIndicator(List<IndicatorImportModel> importValues, List<object> errorRow = null, string username = null);
        ResultModel ImportIndicator(AggregatedData aggregatedData, string username = null);
        ResultModel ClearAll();
        ResultModel Recall(RecallModel recallModel);
    }

    public class AggregatedValueService : IAggregatedValueService
    {
        private const string INDICATOR_DENOMINATOR = nameof(IndicatorElasticModel.Denominator);
        private const string INDICATOR_NUMERATOR = nameof(IndicatorElasticModel.Numerator);


        private readonly AppDBContext _dbContext;
        private readonly ElasticClient _elasticClient;

        private IConfiguration Configuration { get; }
        private string _aggregatedValueIndex { get; set; }

        public AggregatedValueService(AppDBContext dBContext, ElasticClient elasticClient, IConfiguration configuration)
        {
            _dbContext = dBContext;
            _elasticClient = elasticClient;
            Configuration = configuration;
            _aggregatedValueIndex = Configuration["elasticsearch:index"];
        }

        public ResultModel GetAggregatedValues(int year, int? quarter, int? month, string indicatorGroup, string indicatorCode, string groupBy, string provinceCode, string districtCode, Guid? indicatorGroupId = null)
        {
            var result = new ResultModel();
            result.Succeed = true;
            try
            {
                var dims = groupBy == "AgeGroup" ? _dbContext.AgeGroups.ToList().Select(s => s.Adapt<DimensionViewModel>())
                    : groupBy == "KeyPopulation" ? _dbContext.KeyPopulations.ToList().Select(s => s.Adapt<DimensionViewModel>())
                    : groupBy == "Gender" ? _dbContext.Gender.ToList().Select(s => s.Adapt<DimensionViewModel>())
                    : _dbContext.Sites.ToList().Select(s => s.Adapt<DimensionViewModel>());
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
            var districts = _dbContext.Districts.Where(d => d.Province.Code == provinceCode && (string.IsNullOrEmpty(districtCode) || _districts.Contains(d.Code))).Select(s => s.Id);
            var sites = _dbContext.Sites.Where(s => districts.Contains(s.DistrictId)).Select(s => s.Id);
            var limit = month == null ? year * 100 + toMonth : year * 100 + month;
            var aggregatedValues = _dbContext.AggregatedValues.Where(w => (w.Month + w.Year * 100 <= limit) && sites.Contains(w.SiteId) && w.Indicator.Name == indicator);
            if (!string.IsNullOrEmpty(ageGroups))
            {
                var _ageGroups = ageGroups.Split(',').Select(s => Guid.Parse(s));
                aggregatedValues = aggregatedValues.Where(s => s.AgeGroupId != null && _ageGroups.Contains(s.AgeGroupId.Value));
            }
            if (!string.IsNullOrEmpty(keyPopulations))
            {
                var _keyPopulations = keyPopulations.Split(',').Select(s => Guid.Parse(s));
                aggregatedValues = aggregatedValues.Where(s => s.KeyPopulationId != null && _keyPopulations.Contains(s.KeyPopulationId.Value));
            }
            if (!string.IsNullOrEmpty(genders))
            {
                var _genders = genders.Split(',').Select(s => Guid.Parse(s));
                aggregatedValues = aggregatedValues.Where(s => s.GenderId != null && _genders.Contains(s.GenderId.Value));
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
            , string ageGroups = "", string keyPopulations = "", string genders = "", string sites = "", Guid? indicatorGroupId = null)
        {
            var _districts = !string.IsNullOrEmpty(districtCode) ? districtCode.Split(',').ToList() : new List<string>();
            var _ageGroups = !string.IsNullOrEmpty(ageGroups) ? ageGroups.Split(',').ToList() : new List<string>();
            var _keyPopulations = !string.IsNullOrEmpty(keyPopulations) ? keyPopulations.Split(',').ToList() : new List<string>();
            var _genders = !string.IsNullOrEmpty(genders) ? genders.Split(',').ToList() : new List<string>();
            var _sites = !string.IsNullOrEmpty(sites) ? sites.Split(',').ToList() : new List<string>();

            var indicators = _dbContext.Indicators.Include(i => i.IndicatorGroup).ToList();
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

        public ResultModel PopulateMaskData()
        {
            var maskData = new List<IndicatorElasticModel>();
            for (var i = 1; i < 13; i++)
            {
                var monthView = i < 10 ? "0" + i : i + "";
                var quarter = i < 4 ? 1 : i < 7 ? 2 : i < 10 ? 3 : 4;
                var quarterView = "" + quarter;
                maskData.Add(new IndicatorElasticModel
                {
                    MonthView = monthView,
                    LastDenominator = 0,
                    LastNumerator = 0,
                    Year = DateTime.Now.Year,
                    Numerator = 0,
                    Denominator = 0,
                    Date = DateTime.Now,
                    IsTotal = false,
                    Month = i,
                    Quarter = quarter,
                    QuarterView = quarterView,
                    ValueType = 1,
                    YearView = "" + DateTime.Now.Year
                });
            }
            _elasticClient.IndexMany(maskData, _aggregatedValueIndex);
            return new ResultModel()
            {
                Succeed = true
            };
        }

        public ResultModel PopulateDrugsSafetyMoS()
        {
            var data = _dbContext.AggregatedValues.Where(s => s.IsDeleted == false && s.Indicator.Name == "drugs_safety_mos" && s.Month != null).ToList();
            var group_by_year = data.GroupBy(s => s.Year).ToList();
            var populateData = new List<IndicatorElasticModel>();
            foreach (var year in group_by_year)
            {
                var group_by_month = year.GroupBy(s => s.Month).ToList();
                foreach (var month in group_by_month)
                {
                    var group_by_province = month.GroupBy(s => s.Site.District.Province).ToList();
                    foreach (var province in group_by_province)
                    {
                        var group_by_drug = province.GroupBy(s => s.DrugName).ToList();
                        foreach (var drug in group_by_drug)
                        {
                            var _year = year.Key;
                            var _month = month.Key;
                            var monthView = _month > 9 ? _month + "" : "0" + _month;
                            var _drug = drug.Key;
                            var isSafe = drug.Sum(s => (s.Numerator - s.Denominator)) >= 0;
                            var item = new IndicatorElasticModel
                            {
                                PeriodType = "month",
                                Year = _year,
                                YearView = _year + "",
                                Month = _month,
                                MonthView = monthView,
                                IndicatorCode = "%ARV_2MoS",
                                IndicatorName = "%ARV_2MoS",
                                IndicatorGroup = "Drugs",
                                IsSafe = isSafe,
                                ProvinceCode = province.Key.Code,
                                ProvinceName = province.Key.Name,
                                IsMaskData = true,
                                Drug = _drug,
                            };
                            populateData.Add(item);
                        }
                    }
                }
            };
            populateData.ForEach(s =>
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
                else if (s.PeriodType.ToLower() == "day")
                {
                    s.Date = new DateTime(s.Year, s.Month.Value, s.Day.Value);
                }
            });

            var pageSize = 100;
            var pageCount = populateData.Count / pageSize + (populateData.Count % pageSize != 0 ? 1 : 0);
            for (var index = 0; index < pageCount; index++)
            {
                var response = _elasticClient.IndexMany(populateData.Skip(pageSize * index).Take(pageSize), _aggregatedValueIndex);
            }
            return new ResultModel
            {
                Succeed = true,
            };
        }

        public ResultModel PopulateData(string indicator, int year, int month, int? day = null, bool all = false, bool makeDeletion = false)
        {
            if (makeDeletion)
            {
                _elasticClient.DeleteByQuery<IndicatorElasticModel>(del => del.Index(_aggregatedValueIndex).Query(q => q.QueryString(qs => qs.Query("*"))));
            }
            var periods = new List<string>
            {
                "day",
                "month",
                "quarter",
                "year"
            };
            var data = _dbContext.AggregatedValues
                                .Where(w => w.IsValid)
                                .Where(w => all ||
                                            (w.Month == month && w.Year == year
                                            && (string.IsNullOrEmpty(indicator) || w.Indicator.Name == indicator)));

            var populateData = new List<IndicatorElasticModel>();

            var indicators = data.ToList().GroupBy(s => s.IndicatorId);
            var maxMonthDic = new Dictionary<Guid, int>();
            foreach (var item in indicators)
            {
                var max = item.OrderByDescending(s => s.Year).ThenByDescending(s => s.Month).FirstOrDefault();
                var _ = max.PeriodType == "month" ? max.Year * 100 + max.Month.Value : max.Year * 100 + max.Quarter.Value;
                maxMonthDic.Add(item.Key, _);
            }

            foreach (var s in data)
            {
                #region Update Last Period
                //begin calaculating last date
                DateTime date = new DateTime();
                var periodType = s.PeriodType.ToLower();
                var periodIndex = periods.IndexOf(periodType);
                if (s.PeriodType.ToLower() == "month")
                {
                    date = new DateTime(s.Year, s.Month.Value, DateTime.DaysInMonth(s.Year, s.Month.Value));
                }
                else if (s.PeriodType.ToLower() == "quarter")
                {
                    var _day = s.Quarter.Value == 1 ? 31 : s.Quarter.Value == 2 ? 30 : s.Quarter.Value == 3 ? 30 : 31;
                    var _month = s.Quarter.Value == 1 ? 3 : s.Quarter.Value == 2 ? 6 : s.Quarter.Value == 3 ? 9 : 12;
                    date = new DateTime(s.Year, _month, _day);
                }
                else if (s.PeriodType.ToLower() == "day")
                {
                    date = new DateTime(s.Year, s.Month.Value, s.Day.Value);
                }

                DateTime lastDate = new DateTime();

                if (periodType == "day")
                {
                    lastDate = date.AddDays(-1);
                }
                else if (periodType == "month")
                {
                    lastDate = date.AddMonths(-1);
                }
                else if (periodType == "quarter")
                {
                    lastDate = date.AddMonths(-3);
                }
                else if (periodType == "year")
                {
                    lastDate = date.AddYears(-1);
                };
                //end calculating last date
                int lastYear = lastDate.Year, lastQuarter = lastDate.Quarter(), lastMonth = lastDate.Month, lastDay = lastDate.Day;
                var lastData = _dbContext.AggregatedValues
                                         .FirstOrDefault(f => f.SiteId == s.SiteId
                                                        && f.KeyPopulationId == s.KeyPopulationId
                                                        && f.AgeGroupId == s.AgeGroupId
                                                        && f.GenderId == s.GenderId
                                                        && f.IndicatorId == s.IndicatorId
                                                        && (periodIndex > 0 || f.Day == lastDay)
                                                        && (periodIndex > 1 || f.Month == lastMonth)
                                                        && (periodIndex > 2 || f.Quarter == lastQuarter)
                                                        && f.Year == lastYear
                                                        && f.PeriodType == s.PeriodType);
                var lastN = lastData != null ? lastData.Numerator : 0;
                var lastD = lastData != null ? lastData.Denominator : 0;
                var monthView = periodType == "month" ? (s.Month < 10 ? "0" + s.Month : s.Month + "") : "_";
                var quarterView = periodType == "quarter" ? s.Quarter + "" : "_";
                var isSafe = s.Numerator >= s.Denominator;

                maxMonthDic.TryGetValue(s.IndicatorId, out int max);
                var current = s.PeriodType == "month" ? s.Year * 100 + s.Month.Value : s.Year * 100 + s.Quarter.Value;
                if (current <= max)
                {
                    populateData.Add(new IndicatorElasticModel
                    {
                        IndicatorName = s.Indicator.Name,
                        IndicatorCode = s.Indicator.Code,
                        IndicatorGroup = s.Indicator.IndicatorGroup.Name,
                        IsTotal = s.Indicator.IsTotal.Value,
                        Quarter = s.Quarter,
                        DistrictCode = s.Site.District.Code,
                        DistrictName = s.Site.District.NameWithType,
                        ProvinceCode = s.Site.District.Province.Code,
                        ProvinceName = s.Site.District.Province.NameWithType,
                        Location = (s.Site.Lat != null && s.Site.Lng != null) ? new GeoCoordinate(s.Site.Lat.Value, s.Site.Lng.Value) : null,
                        ValueType = s.DataType == DataType.Number ? 1 : 2,
                        Denominator = s.Denominator,
                        Numerator = s.Numerator,
                        LastDenominator = lastD,
                        LastNumerator = lastN,
                        AgeGroup = s.AgeGroup.Name,
                        KeyPopulation = s.KeyPopulation.Name,
                        Site = s.Site.Name,
                        Gender = s.Gender.Name,
                        PeriodType = s.PeriodType,

                        Month = !all ? month : s.Month,
                        Year = !all ? year : s.Year,
                        Day = !all ? day : s.Day,

                        YearView = s.Year + "",
                        QuarterView = quarterView,
                        MonthView = monthView,
                        Drug = s.DrugName,
                        IsSafe = isSafe,
                        DataSource = s.DataSource
                    });
                }
                #endregion
            }

            foreach (var s in data)
            {
                #region Update Next Period (fix missing data)
                //begin calaculating next date
                DateTime date = new DateTime();
                var periodType = s.PeriodType.ToLower();
                var periodIndex = periods.IndexOf(periodType);
                if (s.PeriodType.ToLower() == "month")
                {
                    date = new DateTime(s.Year, s.Month.Value, DateTime.DaysInMonth(s.Year, s.Month.Value));
                }
                else if (s.PeriodType.ToLower() == "quarter")
                {
                    var _day = s.Quarter.Value == 1 ? 31 : s.Quarter.Value == 2 ? 30 : s.Quarter.Value == 3 ? 30 : 31;
                    var _month = s.Quarter.Value == 1 ? 3 : s.Quarter.Value == 2 ? 6 : s.Quarter.Value == 3 ? 9 : 12;
                    date = new DateTime(s.Year, _month, _day);
                }
                else if (s.PeriodType.ToLower() == "day")
                {
                    date = new DateTime(s.Year, s.Month.Value, s.Day.Value);
                }
                var nextDate = DateTime.Now;
                if (periodType == "day")
                {
                    nextDate = date.AddDays(1);
                }
                else if (periodType == "month")
                {
                    nextDate = date.AddMonths(1);
                }
                else if (periodType == "quarter")
                {
                    nextDate = date.AddMonths(3);
                }
                else if (periodType == "year")
                {
                    nextDate = date.AddYears(1);
                };
                int nextYear = nextDate.Year, nextQuarter = nextDate.Quarter(), nextMonth = nextDate.Month, nextDay = nextDate.Day;
                var nextData = _dbContext.AggregatedValues
                                         .FirstOrDefault(f => f.SiteId == s.SiteId
                                                        && f.KeyPopulationId == s.KeyPopulationId
                                                        && f.AgeGroupId == s.AgeGroupId
                                                        && f.GenderId == s.GenderId
                                                        && f.IndicatorId == s.IndicatorId
                                                        && (periodIndex > 0 || f.Day == nextDay)
                                                        && (periodIndex > 1 || f.Month == nextMonth)
                                                        && (periodIndex > 2 || f.Quarter == nextQuarter)
                                                        && f.Year == nextYear
                                                        && f.PeriodType == s.PeriodType);
                if (nextData == null)
                {
                    var _periodType = s.PeriodType.ToLower();
                    var _month = nextDate.Month;
                    var monthView = _periodType == "month" ? (_month < 10 ? "0" + _month : _month + "") : "_";
                    var yearView = nextDate.Year + "";
                    var quarterView = _periodType == "quarter" ? (_month < 4 ? "1" : _month < 7 ? "2" : _month < 10 ? "3" : "4") : "_";
                    var isSafe = s.Numerator >= s.Denominator;
                    maxMonthDic.TryGetValue(s.IndicatorId, out int max);
                    var current = s.PeriodType == "month" ? nextDate.Year * 100 + nextDate.Month : s.Year * 100 + nextDate.Quarter();
                    if (current <= max)
                    {
                    }
                    populateData.Add(new IndicatorElasticModel
                    {
                        IndicatorName = s.Indicator.Name,
                        IndicatorCode = s.Indicator.Code,
                        IndicatorGroup = s.Indicator.IndicatorGroup.Name,
                        IsTotal = s.Indicator.IsTotal.Value,
                        Quarter = nextQuarter,
                        DistrictCode = s.Site.District.Code,
                        DistrictName = s.Site.District.NameWithType,
                        ProvinceCode = s.Site.District.Province.Code,
                        ProvinceName = s.Site.District.Province.NameWithType,
                        Location = null,
                        ValueType = s.DataType == DataType.Number ? 1 : 2,
                        Denominator = 0,  // data will be not existed on next period
                        Numerator = 0,    //
                        LastDenominator = s.Denominator,
                        LastNumerator = s.Numerator,
                        AgeGroup = s.AgeGroup.Name,
                        KeyPopulation = s.KeyPopulation.Name,
                        Site = s.Site.Name,
                        Gender = s.Gender.Name,
                        PeriodType = s.PeriodType,
                        Month = nextMonth,
                        Year = nextYear,
                        Day = nextDay,
                        MonthView = monthView,
                        YearView = yearView,
                        QuarterView = quarterView,
                        Drug = s.DrugName,
                        IsSafe = isSafe,
                        DataSource = s.DataSource,
                    });
                }
                #endregion
            }

            populateData.ForEach(s =>
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
                else if (s.PeriodType.ToLower() == "day")
                {
                    s.Date = new DateTime(s.Year, s.Month.Value, s.Day.Value);
                }
            });

            var total = 0;
            var pageSize = 500;
            var pageCount = populateData.Count / pageSize + (populateData.Count % pageSize != 0 ? 1 : 0);
            for (var index = 0; index < pageCount; index++)
            {
                var part = populateData.Skip(pageSize * index).Take(pageSize);
                var response = _elasticClient.IndexMany(part, _aggregatedValueIndex);
                if (!response.IsValid)
                {
                    Console.WriteLine(response.DebugInformation);
                }
                total += part.Count();
            }

            if (makeDeletion)
            {
                PopulateDrugsPlan();
                PopulateDrugsSafetyMoS();
                PopulateMaskData();
            }

            return new ResultModel()
            {
                Succeed = true,
                Data = new
                {
                    _total = total
                }
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
                data.Quarter = (new DateTime(data.Year, data.Month.Value, 1)).Quarter(); //fix quarter
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
            var dim = _dbContext.Set(typeOfCategory).Adapt<List<Dimension>>()
                .FirstOrDefault(s => !s.IsDeleted && (s.Name == name || s.Code == name));
            if (dim != null)
            {
                return dim.Id;
            }
            else
            {
                var na = _dbContext.Set(typeOfCategory).Adapt<List<Dimension>>().FirstOrDefault(s => s.Name == "N/A");
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
                using var transaction = _dbContext.Database.BeginTransaction();
                errorRows = errorRows != null ? errorRows : new List<object>();
                int total = importValues.Count();
                int succeed = 0;
                int succeedWithUndefinedDimValue = 0;
                int updated = 0;
                var undefinedDimValues = new List<UndefinedDimValue>();
                var permissions = _dbContext.DataPermissions
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
                    var indicator = _dbContext.Indicators.FirstOrDefault(f => !f.IsDeleted && (f.Name.Equals(data.Indicator) || f.Code.Equals(data.Indicator)));
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
                    definedDimValue.TryGetValue("AgeGroup", out Guid ageGroupId);
                    definedDimValue.TryGetValue("Site", out Guid siteId);
                    definedDimValue.TryGetValue("KeyPopulation", out Guid keyPopulationId);
                    definedDimValue.TryGetValue("Gender", out Guid genderId);
                    #region check permission
                    if (username != "admin")
                    {
                        var site = _dbContext.Sites.Include(s => s.District).FirstOrDefault(s => s.Id == siteId);
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

                    #region check site
                    var _site = _dbContext.Sites.Include(s => s.District).FirstOrDefault(s => s.Id == siteId);
                    if (_site == null || _site.Name == "N/A")
                    {
                        errorRows.Add(new
                        {
                            Row = _index,
                            Error = "Undefined Site",
                        });
                        continue;
                    }
                    #endregion

                    #region add aggregated value to database
                    var current = _dbContext.AggregatedValues.FirstOrDefault(f => f.SiteId == siteId
                                                                               && f.KeyPopulationId == keyPopulationId
                                                                               && f.AgeGroupId == ageGroupId
                                                                               && f.GenderId == genderId
                                                                               && f.IndicatorId == indicator.Id
                                                                               && f.Day == data.Day
                                                                               && f.Month == data.Month
                                                                               && f.Quarter == data.Quarter
                                                                               && f.Year == data.Year
                                                                               && f.PeriodType == data.PeriodType);
                    if (indicator.IndicatorGroup.Name == "Drugs")
                    {
                        current = _dbContext.AggregatedValues.FirstOrDefault(f => f.SiteId == siteId
                                                                               && f.DrugName == data.DrugName
                                                                               && f.DrugUnitName == data.DrugUnitName
                                                                               && f.IndicatorId == indicator.Id
                                                                               && f.Day == data.Day
                                                                               && f.Month == data.Month
                                                                               && f.Quarter == data.Quarter
                                                                               && f.Year == data.Year
                                                                               && f.PeriodType == data.PeriodType
                                                                               && f.DataSource == data.DataSource);
                    }
                    if (indicator.IndicatorGroup.Name == "SHI")
                    {
                        current = _dbContext.AggregatedValues.FirstOrDefault(f => f.SiteId == siteId
                                                                               && f.IndicatorId == indicator.Id
                                                                               && f.Day == data.Day
                                                                               && f.Month == data.Month
                                                                               && f.Quarter == data.Quarter
                                                                               && f.Year == data.Year
                                                                               && f.PeriodType == data.PeriodType);
                    }
                    if (current != null)
                    {
                        if (current.UnsolvedDimValues != null && current.UnsolvedDimValues.Count == 0 && localUndefinedDimValues.Count == 0)
                        {
                            current.Numerator = data.Numerator;
                            current.Denominator = data.Denominator != null ? data.Denominator.Value : 0;
                            _dbContext.AggregatedValues.Update(current);
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
                            DrugName = data.DrugName,
                            DrugUnitName = data.DrugUnitName,
                            DataSource = data.DataSource,
                        };
                        _dbContext.AggregatedValues.Add(aggregatedValue);
                        if (localUndefinedDimValues.Count > 0)
                        {
                            succeedWithUndefinedDimValue++;
                            localUndefinedDimValues.ToList().ForEach(u =>
                            {
                                var undefinedDimValue = _dbContext.UndefinedDimValues.FirstOrDefault(s => s.Dimension == u.Value.Dimension
                                                                            && s.UndefinedValue == u.Value.UndefinedValue);
                                if (undefinedDimValue == null)
                                {
                                    undefinedDimValue = u.Value;
                                    _dbContext.UndefinedDimValues.Add(undefinedDimValue);
                                }
                                _dbContext.UnsolvedDimValues.Add(new UnsolvedDimValue()
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
                _dbContext.SaveChanges();

                transaction.Commit();
                using (var _httpClient = new HttpClient())
                {
                    _httpClient.PostAsync("https://pqm-core.hcdc.vn/api/AggregatedValues/PopulateData?all=true&makeDeletion=true", null);
                }
                return new ResultModel()
                {
                    Succeed = true,
                    Error = null,
                    Data = new
                    {
                        total,
                        succeed,
                        updated,
                        error_count = (total - succeed - updated),
                        error_rows = errorRows
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
                    if (string.IsNullOrEmpty(period))
                    {
                        break;
                    }
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
            , Guid? provinceId = null, Guid? districId = null, Guid? siteId = null, Guid? indicatorGroupId = null)
        {
            var filter = _dbContext.AggregatedValues.Where(w => true);
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
                filter = filter.Include(w => w.Site).Where(w => w.SiteId == siteId);
            }
            else if (districId != null)
            {
                var sites = _dbContext.Sites.Where(s => s.DistrictId == districId).Select(s => s.Id);
                filter = filter.Where(w => sites.Contains(w.SiteId));
            }
            else if (provinceId != null)
            {
                var districs = _dbContext.Districts.Where(s => s.ProvinceId == provinceId).Select(s => s.Id);
                var sites = _dbContext.Sites.Where(s => districs.Contains(s.DistrictId)).Select(s => s.Id);
                filter = filter.Where(w => sites.Contains(w.SiteId));
            }
            if (indicatorGroupId != null)
            {
                filter = filter.Include(w => w.Indicator).Where(w => w.Indicator.IndicatorGroupId == indicatorGroupId);
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
            var indicator = _dbContext.Indicators.FirstOrDefault(f => f.Name.Equals(aggregatedValue.Indicator));
            Guid ageGroupId; definedDimValue.TryGetValue("AgeGroup", out ageGroupId);
            Guid siteId; definedDimValue.TryGetValue("Site", out siteId);
            Guid keyPopulationId; definedDimValue.TryGetValue("KeyPopulation", out keyPopulationId);
            Guid genderId; definedDimValue.TryGetValue("Gender", out genderId);
            var current = _dbContext.AggregatedValues.FirstOrDefault(f => f.SiteId == siteId
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
                    DrugName = aggregatedValue.DrugName,
                    DrugUnitName = aggregatedValue.DrugUnitName,
                    DataSource = aggregatedValue.DataSource,
                };
                _dbContext.AggregatedValues.Add(_aggregatedValue);
            }
            #endregion
            rs.Succeed = _dbContext.SaveChanges() > 0;
            return rs;
        }

        public ResultModel Delete(Guid id)
        {
            var aggregatedValue = _dbContext.AggregatedValues.FirstOrDefault(s => s.Id == id);
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
                _dbContext.UnsolvedDimValues.Remove(u);
            }
            _dbContext.AggregatedValues.Remove(aggregatedValue);
            var rs = new ResultModel()
            {
                Succeed = _dbContext.SaveChanges() > 0
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
            var indicator = _dbContext.Indicators.FirstOrDefault(f => f.Name.Equals(aggregatedValue.Indicator));
            Guid ageGroupId; definedDimValue.TryGetValue("AgeGroup", out ageGroupId);
            Guid siteId; definedDimValue.TryGetValue("Site", out siteId);
            Guid keyPopulationId; definedDimValue.TryGetValue("KeyPopulation", out keyPopulationId);
            Guid genderId; definedDimValue.TryGetValue("Gender", out genderId);
            var current = _dbContext.AggregatedValues.FirstOrDefault(f => f.SiteId == siteId
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
                _dbContext.AggregatedValues.Update(current);
            }
            #endregion
            rs.Succeed = _dbContext.SaveChanges() > 0;
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
            var _indicators = _dbContext.Indicators;
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
                if (!double.TryParse(row.data.value, out double numerator))
                {
                    _errorRows.Add(new
                    {
                        Row = i + 1,
                        Error = string.Format("Value is not defined")
                    });
                    continue;
                };
                var data = new IndicatorImportModel()
                {
                    RowIndex = i,
                    AgeGroup = row.data.age_group,
                    KeyPopulation = row.data.key_population,
                    Gender = row.data.sex,
                    Site = row.site_code,
                    Numerator = (int)numerator,
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
                if (row.indicator_code == "%ARV_Consu_Plan" || row.indicator_code == "%ARV_2MoS" || row.indicator_code == "drugs_plan" || row.indicator_code == "drugs_safety_mos")
                {
                    var optional_data = row.optional_data;
                    if (!double.TryParse(optional_data.value, out double denominator))
                    {
                        _errorRows.Add(new
                        {
                            Row = i + 1,
                            Error = string.Format("Value is not defined")
                        });
                        continue;
                    };
                    data.Denominator = (int)denominator;
                    data.DrugName = optional_data.drug_name;
                    data.DrugUnitName = optional_data.unit_name;
                    data.DataSource = optional_data.data_source;
                }
                if (indicator.IndicatorGroup.Name == "Drugs" || indicator.IndicatorGroup.Name == "SHI")
                {
                    //set N/A for age group, gender, key population
                    data.Gender = "N/A";
                    data.AgeGroup = "N/A";
                    data.KeyPopulation = "N/A";
                }
                importData.Add(data);
            };
            if (importData.Count() == 0)
            {
                return new ResultModel
                {
                    Succeed = false,
                    Error = new ErrorModel
                    {
                        ErrorMessage = JsonConvert.SerializeObject(_errorRows)
                    }
                };
            }
            return ImportIndicator(importData, _errorRows, username);
        }

        public ResultModel ClearAll()
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                _elasticClient.DeleteByQuery<IndicatorElasticModel>(del => del.Index(_aggregatedValueIndex).Query(q => q.QueryString(qs => qs.Query("*"))));
                _dbContext.UnsolvedDimValues.RemoveRange(_dbContext.UnsolvedDimValues);
                _dbContext.UndefinedDimValues.RemoveRange(_dbContext.UndefinedDimValues);
                _dbContext.AggregatedValues.RemoveRange(_dbContext.AggregatedValues);
                _dbContext.SaveChanges();
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

        public ResultModel Recall(RecallModel recallModel)
        {
            var rs = new ResultModel
            {
                Succeed = true
            };
            try
            {
                using var transaction = _dbContext.Database.BeginTransaction();
                var filter = _dbContext
                    .AggregatedValues.Where(w => w.Year == recallModel.Year); //1
                if (recallModel.Month != null)
                {
                    filter = filter.Where(w => w.Month == recallModel.Month.Value); //2
                }
                if (recallModel.Quarter != null)
                {
                    filter = filter.Where(w => w.Quarter == recallModel.Quarter.Value); //3
                }
                if (!string.IsNullOrEmpty(recallModel.IndicatorCode))
                {
                    filter = filter.Where(w => w.Indicator.Code == recallModel.IndicatorCode || w.Indicator.Name == recallModel.IndicatorCode); //4
                }
                if (!string.IsNullOrEmpty(recallModel.AgeGroup))
                {
                    filter = filter.Where(w => w.AgeGroup.Name == recallModel.AgeGroup); //5
                }
                if (!string.IsNullOrEmpty(recallModel.Drug))
                {
                    filter = filter.Where(w => w.DrugName == recallModel.Drug); //6
                }
                if (!string.IsNullOrEmpty(recallModel.Gender))
                {
                    filter = filter.Where(w => w.Gender.Name == recallModel.Gender); //7
                }
                if (!string.IsNullOrEmpty(recallModel.KeyPopulation))
                {
                    filter = filter.Where(w => w.KeyPopulation.Name == recallModel.KeyPopulation); //8
                }
                if (!string.IsNullOrEmpty(recallModel.Site))
                {
                    filter = filter.Where(w => w.Site.Code == recallModel.Site); //9
                }
                var removed_data = filter.ToArray();
                _dbContext.AggregatedValues.RemoveRange(removed_data);
                _dbContext.SaveChanges();
                transaction.Commit();
                using (var _httpClient = new HttpClient())
                {
                    _httpClient.PostAsync("https://pqm-core.hcdc.vn/api/AggregatedValues/PopulateData?all=true&makeDeletion=true", null);
                }
                rs.Data = new
                {
                    total = removed_data.Count()
                };
            }
            catch (Exception ex)
            {
                rs.Succeed = false;
                rs.Error.ErrorMessage = ex.Message;
            }
            return rs;
        }

        public ResultModel PopulateDrugsPlan()
        {
            var data = _dbContext.AggregatedValues.Where(s => s.Indicator.Code == "%ARV_Consu_Plan" && !s.IsDeleted);
            var populateData = new List<IndicatorElasticModel>();
            data.ToList().ForEach(s =>
            {
                var from = s.Quarter == 1 ? 1 : s.Quarter == 2 ? 4 : s.Quarter == 3 ? 7 : 10;
                var to = from + 3;
                for (int month = from; month < to; month++)
                {
                    var monthView = month < 10 ? "0" + month : month + "";
                    var item = new IndicatorElasticModel
                    {
                        Year = s.Year,
                        YearView = s.Year + "",
                        Month = month,
                        MonthView = monthView,
                        Numerator = s.Numerator,
                        Denominator = s.Denominator,
                        Drug = s.DrugName,
                        IndicatorCode = "%ARV_Consu_Plan",
                        IndicatorGroup = "Drugs",
                        IsTotal = true,
                        Site = s.Site.Name,
                        DistrictCode = s.Site.District.Code,
                        DistrictName = s.Site.District.Name,
                        ProvinceCode = s.Site.District.Province.Code,
                        ProvinceName = s.Site.District.Province.Name,
                        IndicatorName = "%ARV_Consu_Plan",
                        PeriodType = "month",
                        IsMaskData = true,
                    };
                    populateData.Add(item);
                }
            });
            populateData.ForEach(s =>
            {
                if (s.PeriodType.ToLower() == "month")
                {
                    s.Date = new DateTime(s.Year, s.Month.Value, 1);
                }
                else if (s.PeriodType.ToLower() == "quarter")
                {
                    var _day = s.Quarter.Value == 1 ? 31 : s.Quarter.Value == 2 ? 30 : s.Quarter.Value == 3 ? 30 : 31;
                    var _month = s.Quarter.Value == 1 ? 3 : s.Quarter.Value == 2 ? 6 : s.Quarter.Value == 3 ? 9 : 12;
                    s.Date = new DateTime(s.Year, _month, _day);
                }
                else if (s.PeriodType.ToLower() == "day")
                {
                    s.Date = new DateTime(s.Year, s.Month.Value, s.Day.Value);
                }
            });

            var pageSize = 100;
            var pageCount = populateData.Count / pageSize + (populateData.Count % pageSize != 0 ? 1 : 0);
            for (var index = 0; index < pageCount; index++)
            {
                var response = _elasticClient.IndexMany(populateData.Skip(pageSize * index).Take(pageSize), _aggregatedValueIndex);
            }

            return new ResultModel
            {
                Succeed = true,
            };
        }
    }
}
