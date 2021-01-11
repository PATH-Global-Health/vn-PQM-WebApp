using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class IndicatorGroup : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<Indicator> Indicators { get; set; }
    }
}
