using System;
namespace PQM_WebApp.Data.ViewModels
{
    public class ThresholdSettingCreateModel
    {
        public string Username { get; set; }
        public string DistrictCode { get; set; }
        public string ProvinceCode { get; set; }
        public string IndicatorName { get; set; }
        public double From { get; set; }
        public double To { get; set; }
        public string ColorCode { get; set; }
        public int Type { get; set; }
    }

    public class ThresholdSettingViewModel : ThresholdSettingCreateModel
    {
        public Guid Id { get; set; }
    }
}
