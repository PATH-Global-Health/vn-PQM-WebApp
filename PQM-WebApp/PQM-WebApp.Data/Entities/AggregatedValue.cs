using PQM_WebApp.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PQM_WebApp.Data.Entities
{

    public class AggregatedValue : BaseEntity
    {
        public DataType DataType { get; set; }
        public int Denominator { get; set; }
        public int Numerator { get; set; }
        //Time Dimension
        public string PeriodType { get; set; } //Day, Month, Quarter, Year
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
        //Location Dimension
        public Guid SiteId { get; set; }
        [ForeignKey("SiteId")]
        public virtual Site Site { get; set; }
        public Guid IndicatorId { get; set; }
        [ForeignKey("IndicatorId")]
        public virtual Indicator Indicator { get; set; }
        //Gender Dimension
        public Guid GenderId { get; set; }
        [ForeignKey("GenderId")]
        public virtual Gender Gender { get; set; }
        //Age Group Dimension
        public Guid AgeGroupId { get; set; }
        [ForeignKey("AgeGroupId")]
        public virtual AgeGroup AgeGroup { get; set; }
        //Key population Dimension
        public Guid KeyPopulationId { get; set; }
        [ForeignKey("KeyPopulationId")]
        public virtual KeyPopulation KeyPopulation { get; set; }
        public virtual ICollection<UnsolvedDimValue> UnsolvedDimValues { get; set; }
    }
}
