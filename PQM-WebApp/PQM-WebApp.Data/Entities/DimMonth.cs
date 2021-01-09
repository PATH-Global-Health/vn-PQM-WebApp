using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class DimMonth
    {
        [Key, Column(Order = 0)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte MonthNumOfYear { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid QuarterId { get; set; }
        [ForeignKey("QuarterId")]
        public virtual DimQuarter Quarter { get; set; }
        public virtual ICollection<DimWeek> Weeks { get; set; }
    }
}
