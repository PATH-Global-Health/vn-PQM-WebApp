using System;
using System.Collections.Generic;

namespace PQM_WebApp.Data.ViewModels
{
    public class IndicatorDataObject
    {
        public string sex { get; set; }
        public string age_group { get; set; }
        public string key_population { get; set; }
        public string type { get; set; }
        public string value { get; set; }
        public int? denominatorValue { get; set; }
    }

    public class ProvinceIndicatorObject
    {
        public string indicator_code { get; set; }
        public string district_code { get; set; }
        public string site_code { get; set; }
        public IndicatorDataObject data { get; set; }
        public DrugIndicatorDataObject optional_data { get; set; }
    }

    public class AggregatedData
    {
        public string province_code { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public List<ProvinceIndicatorObject> datas { get; set; }
    }

    public class DrugIndicatorDataObject
    {
        public string value { get; set; }
        public string drug_name { get; set; }
        public string unit_name { get; set; }
        public string data_source { get; set; }
    }
}
