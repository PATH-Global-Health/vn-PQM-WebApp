using System;
namespace PQM_WebApp.Data.Models
{
    public class IndicatorElasticModel
    {
        public string Name { get; set; }
        //1: number; 2: percent
        public int ValueType { get; set; }
        public int? Value { get; set; }
        public int? Denominator { get; set; }
        public int? Numerator { get; set; }
        public string AgeGroup { get; set; }
        public string KeyPopulation { get; set; }
        public string Gender { get; set; }
        public string Site { get; set; }
        //time
        public int Year { get; set; }
        public int Month { get; set; }
        public int? Day { get; set; }
    }
}
