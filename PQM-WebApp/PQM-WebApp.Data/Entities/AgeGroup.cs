using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class AgeGroup : BaseEntity
    {
        public string Name { get; set; }
        public byte From { get; set; }
        public byte To { get; set; }

        public virtual ICollection<AggregatedValue> AggregatedValues { get; set; }
    }
}
