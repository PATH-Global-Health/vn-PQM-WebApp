using PQM_WebApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.Models
{

    public class IndicatorValue
    {
        public int Value { get; set; }
        public int? Denominator { get; set; }
        public int? Numerator { get; set; }
        public DataType DataType { get; set; }
        public string CriticalInfo { get; set; }
    }

    public class IndicatorTrend
    {
        public int Direction { get; set; }
        public string CriticalInfo { get; set; }
        public double ComparePercent { get; set; }
    }

    public class IndicatorModel
    {
        public int Order { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }
        public IndicatorValue Value { get; set; }
        public IndicatorTrend Trend { get; set; }
        public string CriticalInfo { get; set; }

    }

    public class RecallModel
    {
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }

        public string IndicatorCode { get; set; }
        public string AgeGroup { get; set; }
        public string Gender { get; set; }
        public string KeyPopulation { get; set; }
        public string Site { get; set; }
        public string Drug { get; set; }
    }
}
