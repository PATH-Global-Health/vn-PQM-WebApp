using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class IndicatorCreateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public string CreatedBy { get; set; }
        public bool IsTotal { get; set; }
        public Guid IndicatorGroupId { get; set; }
    }

    public class IndicatorViewModel : IndicatorCreateModel
    {
        public Guid Id { get; set; }
    }
}
