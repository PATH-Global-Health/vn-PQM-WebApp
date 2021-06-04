using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class UnsolvedDimValue
    {
        public Guid AggregatedValueId { get; set; }
        public Guid UndefinedDimValueId { get; set; }

        [ForeignKey("AggregatedValueId")]
        public virtual AggregatedValue AggregatedValue { get; set; }
        [ForeignKey("UndefinedDimValueId")]
        public virtual UndefinedDimValue UndefinedDimValue { get; set; }
    }
}
