using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class IndicatorSummaryValue
    {
        [Key, Column(Order = 0)]
        public Guid Id { get; set; }
        public Guid IndicatorId { get; set; }
        public Guid? WardId { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid CityId { get; set; }
        public int DimDateId { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }

        [ForeignKey("IndicatorId")]
        public virtual Indicator Indicator { get; set; }
        [ForeignKey("WardId")]
        public virtual Ward Ward { get; set; }
        [ForeignKey("DistrictId")]
        public virtual District District { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }
        [ForeignKey("DimDateId")]
        public virtual DimDate DimDate { get; set; }
    }
}
