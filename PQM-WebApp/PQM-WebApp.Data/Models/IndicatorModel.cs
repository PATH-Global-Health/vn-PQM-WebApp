using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.Models
{

    public class IndicatorValue
    {
        public string Value { get; set; }
        public string Type { get; set; }
        public string CriticalInfo { get; set; }
    }

    public class IndicatorTrend
    {
        public int Direction { get; set; }
        public string CriticalInfo { get; set; }
        public int ComparePercent { get; set; }
    }

    public class IndicatorModel
    {
        public string Group { get; set; }
        public string Name { get; set; }
        public IndicatorValue Value { get; set; }
        public IndicatorTrend Trend { get; set; }
        public string CriticalInfo { get; set; }

    }
}
