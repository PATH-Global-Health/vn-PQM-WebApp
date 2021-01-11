using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class DimWeek
    {
        [Key, Column(Order = 0)]
        public Guid Id { get; set; }
        public int WeekNumOfMonth { get; set; }
        public int WeekNumOfQuarter { get; set; }
        public int WeekNumOfYear { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid MonthId { get; set; }
        [ForeignKey("MonthId")]
        public virtual DimMonth Month { get; set; }
        public virtual ICollection<DimDate> Dates { get; set; }
    }
}
