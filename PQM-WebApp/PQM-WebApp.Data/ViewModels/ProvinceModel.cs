using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class ProvinceCreateModel
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Type { get; set; }
        public string NameWithType { get; set; }
        public string Code { get; set; }

        public ICollection<DistrictModel> Districts { get; set; }
    }

    public class ProvinceModel : ProvinceCreateModel
    {
        public Guid Id { get; set; }
    }
}
