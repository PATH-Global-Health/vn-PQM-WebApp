using PQM_WebApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Service
{
    public interface ITestingService
    {
        ResultModel GetIndicators();
    }

    public class TestingService : ITestingService
    {
        public ResultModel GetIndicators()
        {
            return new ResultModel()
            {
                Succeed = true,
                Data = new List<IndicatorModel>()
                {
                    new IndicatorModel()
                    {
                        Group = "Testing",
                        Name = "HTS_TEST_POS",
                        Value = new IndicatorValue()
                        {
                            Value = "141",
                            Type = "Number",
                            CriticalInfo = "red",
                        },
                        CriticalInfo = null,
                        Trend = new IndicatorTrend()
                        {
                            Direction  = 1,
                            CriticalInfo = "red",
                            ComparePercent = 4,
                        }
                    },
                    new IndicatorModel()
                    {
                        Group = "Testing",
                        Name = "HTS_TEST-Recency",
                        Value = new IndicatorValue()
                        {
                            Value = "12",
                            Type = "Number",
                            CriticalInfo = "red",
                        },
                        CriticalInfo = null,
                        Trend = new IndicatorTrend()
                        {
                            Direction  = 1,
                            CriticalInfo = "red",
                            ComparePercent = 9,
                        }
                    },
                    new IndicatorModel()
                    {
                        Group = "Testing",
                        Name = "HTS_TEST_POS successfully refer to OPC",
                        Value = new IndicatorValue()
                        {
                            Value = "12",
                            Type = "number",
                            CriticalInfo = "green",
                        },
                        CriticalInfo = null,
                        Trend = new IndicatorTrend()
                        {
                            Direction  = 1,
                            CriticalInfo = "green",
                            ComparePercent = 4
                        }
                    }
                }
            };
        }
    }
}
