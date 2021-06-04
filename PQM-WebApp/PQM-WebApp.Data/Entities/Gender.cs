using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class Gender : DimensionGroup
    {
        public virtual ICollection<AggregatedValue> AggregatedValues { get; set; }
    }
}
