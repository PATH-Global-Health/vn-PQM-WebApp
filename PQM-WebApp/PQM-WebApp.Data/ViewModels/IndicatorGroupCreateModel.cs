using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class IndicatorGroupViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection <IndicatorViewModel> Indicators { get; set; }
    }

    public class IndicatorGroupCreateModel
    {
        public string Name { get; set; }
    }
}
