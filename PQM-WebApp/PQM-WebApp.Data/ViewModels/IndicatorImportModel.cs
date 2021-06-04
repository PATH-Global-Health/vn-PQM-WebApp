using System;
namespace PQM_WebApp.Data.ViewModels
{
    public class IndicatorImportModel
    {
        public int? RowIndex { get; set; }
        public string PeriodType { get; set; }
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
        public string Indicator { get; set; }
        public string Gender { get; set; }
        public string AgeGroup { get; set; }
        public string KeyPopulation { get; set; }
        public string Site { get; set; }
        public int ValueType { get; set; }
        public int Numerator { get; set; }
        public int? Denominator { get; set; }
    }
}
