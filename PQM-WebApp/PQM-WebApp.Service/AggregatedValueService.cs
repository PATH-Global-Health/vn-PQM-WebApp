﻿using Mapster;
using Microsoft.EntityFrameworkCore;
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
        ResultModel GetAggregatedValues(int year, int quarter, int? month, string indicatorGroup, string indicatorCode, string groupBy);
    }

    public class AggregatedValueService : IAggregatedValueService
    {
        private readonly AppDBContext _dBContext;

        public AggregatedValueService(AppDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public ResultModel GetAggregatedValues(int year, int quarter, int? month, string indicatorGroup, string indicatorCode, string groupBy)
        {
            var result = new ResultModel();
            try
            {
                var fromMonth = quarter == 1 ? 1 : quarter == 2 ? 4 : quarter == 3 ? 7 : 10;
                var toMonth = quarter == 1 ? 3 : quarter == 2 ? 6 : quarter == 3 ? 9 : 12;
                var dimension = groupBy.Adapt<DimensionGroupType>();
                var dimensionString = dimension.Adapt<string>();
                var filter = _dBContext.AggregatedValues
                                        .Where(_ => (month == null ||_.Month.MonthNumOfYear == month)
                                        && _.Month.Year.Year == year
                                        && (fromMonth <= _.Month.MonthNumOfYear && _.Month.MonthNumOfYear <= toMonth)
                                        && _.Indicator.IndicatorGroup.Name == indicatorGroup
                                        && (string.IsNullOrEmpty(indicatorCode) || _.Indicator.Code == indicatorCode))
                                        .Include(_ => dimensionString)
                                        .ToList();
                var grouped = filter.GroupBy(_ => _.GetType().GetProperty(dimensionString).GetValue(_, null))
                                    .OrderBy(_ => (_.Key as DimensionGroup).Order)
                                    .Select(s => new {
                                                    (s.Key as DimensionGroup).Id,
                                                    (s.Key as DimensionGroup).Name,
                                                    Value = s.Sum(v => v.Value),
                                                    Numerator = s.Sum(n => n.Numerator),
                                                    Denominator = s.Sum(d => d.Denominator)
                                                }
                                    );
                result.Data = grouped;
                result.Succeed = true;
            }
            catch (Exception e)
            {
                e.Adapt(result);
            }
            return result;
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
