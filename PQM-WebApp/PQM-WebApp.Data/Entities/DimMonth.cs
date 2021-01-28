﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class DimMonth : BaseEntity
    {
        public string Name { get; set; }
        public byte MonthNumOfYear { get; set; }

        [ForeignKey("YearId")]
        public virtual DimYear Year { get; set; }

        public virtual ICollection<DimDate> Dates { get; set; }
        public virtual ICollection<AggregatedValue> AggregatedValues { get; set; }
    }
}
