using System;
namespace PQM_WebApp.Data.Models
{
    public class IndicatorElasticModel
    {
        //indicator information
        public string IndicatorGroup { get; set; }
        public string IndicatorName { get; set; }
        //group by
        public string AgeGroup { get; set; }
        public string KeyPopulation { get; set; }
        public string Gender { get; set; }
        public string Site { get; set; }
        public string DistrictCode { get; set; } //district of site
        public string ProvinceCode { get; set; } //province of site
        //time dimensions
        public int Year { get; set; }
        public int Quarter { get; set; }
        public int Month { get; set; }
        public int? Day { get; set; }
        //value
        public int ValueType { get; set; } //1: number; 2: percent
        public int? Value { get; set; }
        public int? Denominator { get; set; }
        public int? Numerator { get; set; }
    }
}
