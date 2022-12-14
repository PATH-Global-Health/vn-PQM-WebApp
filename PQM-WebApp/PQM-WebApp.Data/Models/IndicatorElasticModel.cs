using System;
using Nest;

namespace PQM_WebApp.Data.Models
{
    public class IndicatorElasticModel
    {
        public IndicatorElasticModel()
        {
            IsMaskData = false;
        }
        //indicator information
        public string IndicatorGroup { get; set; }
        public string IndicatorName { get; set; }
        public string IndicatorCode { get; set; }
        public bool IsTotal { get; set; }
        //group by
        public string AgeGroup { get; set; }
        public string KeyPopulation { get; set; }
        public string Gender { get; set; }
        public string Drug { get; set; }

        //location dimensions
        public GeoCoordinate Location { get; set; } //for map on kibana
        public string Site { get; set; }
        public string DistrictCode { get; set; } //district of site
        public string DistrictName { get; set; } //for view on kibana
        public string ProvinceCode { get; set; } //province of site
        public string ProvinceName { get; set; } //for view on kibana

        //time dimensions
        public string PeriodType { get; set; }
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
        public DateTime? Date { get; set; }


        //for view on kibana
        public string YearView { get; set; }
        public string QuarterView { get; set; }
        public string MonthView { get; set; }
        public string DayView { get; set; }

        //value
        public int ValueType { get; set; } //1: number; 2: percent
        public int Denominator { get; set; }
        public int Numerator { get; set; }
        //last data for data trend
        public int LastDenominator { get; set; }
        public int LastNumerator { get; set; }

        public bool IsSafe { get; set; }
        public bool IsMaskData { get; set; }

        public string DataSource { get; set; }
    }
}
