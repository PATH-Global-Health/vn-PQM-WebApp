using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class IndicatorGroupCreateModel
    {
        public string Name { get; set; }
        public string CreatedBy { get; set; }
        public ICollection<IndicatorViewModel> Indicators { get; set; }
    }
    public class IndicatorGroupViewModel : IndicatorGroupCreateModel
    {
        public Guid Id { get; set; }
    }

}
