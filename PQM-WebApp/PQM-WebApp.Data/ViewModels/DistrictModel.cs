using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class DistrictCreateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Slug { get; set; }
        public string NameWithType { get; set; }
        public string Path { get; set; }
        public string PathWithType { get; set; }
        public string ParentCode { get; set; }
        public string CreatedBy { get; set; }
        public ICollection<SiteViewModel> Sites { get; set; }
    }

    public class DistrictModel : DistrictCreateModel
    {
        public Guid Id { get; set; }
    }
}
