using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class DimDate : BaseEntity
    {
        public DateTime Date { get; set; }

        public Guid MonthId { get; set; }
        [ForeignKey("MonthId")]
        public virtual DimMonth Month { get; set; }

        public virtual ICollection<AggregatedValue> AggregatedValues { get; set; }
    }
}
