using Mapster;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PQM_WebApp.Data;
using PQM_WebApp.Data.Entities;
using PQM_WebApp.Data.Enums;
using PQM_WebApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PQM_WebApp.Service
{
    public interface IAggregatedValueService
    {
        ResultModel GetAggregatedValues(int year, int month, string indicatorCode, string groupBy);
    }

    public class AggregatedValueService : IAggregatedValueService
    {
        private readonly AppDBContext _dBContext;

        public AggregatedValueService(AppDBContext dBContext)
        {
            _dBContext = dBContext;
        }

        public ResultModel GetAggregatedValues(int year, int month, string indicatorCode, string groupBy)
        {
            var result = new ResultModel();
            try
            {
                var demension = groupBy.Adapt<DimensionGroupType>();
                var demensionString = demension.Adapt<string>();
                var filter = _dBContext.AggregatedValues
                                        .Where(_ => _.Month.MonthNumOfYear == month
                                        && _.Month.Year.Year == year
                                        && _.Indicator.Code == indicatorCode)
                                        .Include(_ => demensionString)
                                        .ToList();
                var grouped = filter.GroupBy(_ => _.GetType().GetProperty(demensionString).GetValue(_, null), 
                                                    _ => _.Value,
                                                    (site, num) => new
                                                    {
                                                        Type = (site as DimensionGroup).Name,
                                                        Num = num.Sum(_ => TryParse(_))
                                                    });
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
