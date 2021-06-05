using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class UndefinedDimValue : BaseEntity
    {
        public string Dimension { get; set; }
        public string UndefinedValue { get; set; }
        public Guid? SourceId { get; set; }
        public virtual ICollection<UnsolvedDimValue> UnsolvedDimValues { get; set; }
    }
}
