using System;
using Nest;

namespace PQM_WebApp.Data.Models
{
    public class IndicatorElasticLocationModel
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class IndicatorElasticModel
    {
        //indicator information
        public string IndicatorGroup { get; set; }
        public string IndicatorName { get; set; }
        public string IndicatorCode { get; set; }
        public bool IsTotal { get; set; }
        //group by
        public string AgeGroup { get; set; }
        public string KeyPopulation { get; set; }
        public string Gender { get; set; }
        //location dimensions
        public GeoCoordinate Location { get; set; }
        public string Site { get; set; }
        public string DistrictCode { get; set; } //district of site
        public string ProvinceCode { get; set; } //province of site
        //time dimensions
        public string PeriodType { get; set; }
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
        public DateTime? Date { get; set; }
        //value
        public int ValueType { get; set; } //1: number; 2: percent
        public int Denominator { get; set; }
        public int Numerator { get; set; }
    }
}
