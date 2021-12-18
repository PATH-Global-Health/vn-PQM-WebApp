using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class SiteCreateModel
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string Code { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public Guid DistrictId { get; set; }
        public Guid SiteTypeId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string CreatedBy { get; set; }
    }

    public class SiteViewModel : SiteCreateModel
    {
        public Guid Id { get; set; }
        public SiteTypeViewModel SiteType { get; set; }
        public DistrictModel District { get; set; }
    }
}
