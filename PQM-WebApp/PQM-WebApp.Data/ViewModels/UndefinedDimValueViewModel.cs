using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class UndefinedDimValueViewModel
    {
        public Guid Id { get; set; }
        public string Dimension { get; set; }
        public string UndefinedValue { get; set; }
        public Guid? SourceId { get; set; }
    }
}
