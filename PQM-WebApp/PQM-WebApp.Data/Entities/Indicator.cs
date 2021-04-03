using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class Indicator : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public bool IsTotal { get; set; }

        public Guid IndicatorGroupId { get; set; }
        [ForeignKey("IndicatorGroupId")]
        public virtual IndicatorGroup IndicatorGroup { get; set; }

        public virtual ICollection<AggregatedValue> AggregatedValues { get; set; }
    }
}
