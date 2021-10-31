using System;
namespace PQM_WebApp.Data.Entities
{
    public class ThresholdSetting : BaseEntity
    {
        public string Username { get; set; }
        public string DistrictCode { get; set; }
        public string ProvinceCode { get; set; }
        public string IndicatorName { get; set; }
        public double From { get; set; }
        public double To { get; set; }
        public string ColorCode { get; set; } //hexa
        public int? Type { get; set; }
    }
}
