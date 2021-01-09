using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class DimDate
    {
        public int Id { get; set; }
        public string DayOfWeekName { get; set; }
        public int DayNumOfWeek { get; set; }
        public int DayNumOfMonth { get; set; }
        public int DayNumOfQuarter { get; set; }
        public int DayNumOfYear { get; set; }
        public int WeekId { get; set; }
        public int MonthId { get; set; }
        public int QuarterId { get; set; }
        public int YearId { get; set; }
        [ForeignKey("WeekId")]
        public virtual DimWeek Week { get; set; }
        [ForeignKey("MonthId")]
        public virtual DimMonth Month { get; set; }
        [ForeignKey("QuarterId")]
        public virtual DimQuarter Quarter { get; set; }
        [ForeignKey("YearId")]
        public virtual DimYear Year { get; set; }
    }
}
