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
        public Guid DistrictId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string CreatedBy { get; set; }
    }

    public class SiteViewModel : SiteCreateModel
    {
        public Guid Id { get; set; }
    }
}
