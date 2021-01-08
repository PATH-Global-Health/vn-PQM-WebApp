using PQM_WebApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PQM_WebApp.Service
{
    public interface IPrEPService
    {
        ResultModel GetIndicators();
        ResultModel GetIndicator(string name);
    }

    public class PrEPService : IPrEPService
    {
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
                            Value = "1,276",
                            Type = "Number",
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
                            Value = "1,925",
                            Type = "Number",
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
                            Value = "92%",
                            Type = "Percent",
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

        public ResultModel GetIndicators()
        {
            return new ResultModel()
            {
                Succeed = true,
                Data = new List<IndicatorModel>()
                {
                    new IndicatorModel()
                    {
                        Group = "PrEP",
                        Name = "PrEP_NEW",
                        Value = new IndicatorValue()
                        {
                            Value = "1,276",
                            Type = "Number",
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
                            Value = "1,925",
                            Type = "Number",
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
                            Value = "92%",
                            Type = "Percent",
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
                }
            };
        }
    }
}
