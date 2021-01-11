using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class DimQuarter
    {
        [Key, Column(Order = 0)]
        public Guid Id { get; set; }
        public byte QuarterNumOfYear { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int YearId { get; set; }
        [ForeignKey("YearId")]
        public virtual DimYear Year { get; set; }
        public virtual ICollection<DimMonth> Months { get; set; }
    }
}
