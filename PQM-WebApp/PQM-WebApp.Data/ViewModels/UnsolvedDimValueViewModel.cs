using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class UnsolvedDimValueViewModel
    {
        public Guid AggregatedValueId { get; set; }
        public Guid UndefinedDimValueId { get; set; }
        public UndefinedDimValueViewModel UndefinedDimValue { get; set; }
    }
}
