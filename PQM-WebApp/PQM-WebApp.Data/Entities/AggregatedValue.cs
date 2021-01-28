using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class AggregatedValue : BaseEntity
    {
        public DataType DataType { get; set; }
        public string Value { get; set; }

        public Guid SiteId { get; set; }
        [ForeignKey("SiteId")]
        public virtual Site Site { get; set; }
        public Guid IndicatorId { get; set; }
        [ForeignKey("IndicatorId")]
        public virtual Indicator Indicator { get; set; }
        public Guid MonthId { get; set; }
        [ForeignKey("MonthId")]
        public virtual DimMonth Month { get; set; }
        public Guid DateId { get; set; }
        [ForeignKey("DateId")]
        public virtual DimDate Date { get; set; }
        public Guid SexId { get; set; }
        [ForeignKey("SexId")]
        public virtual Sex Sex { get; set; }
        public Guid AgeGroupId { get; set; }
        [ForeignKey("AgeGroupId")]
        public virtual AgeGroup AgeGroup { get; set; }
        public Guid KeyPopulationId { get; set; }
        [ForeignKey("KeyPopulationId")]
        public virtual KeyPopulation KeyPopulation { get; set; }
    }
}
