using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class SiteTypeCreateModel
    {
        public string Name { get; set; }
        public string CreatedBy { get; set; }
    }
    
    public class SiteTypeViewModel : SiteTypeCreateModel
    {
        public Guid Id { get; set; }
    }
}
