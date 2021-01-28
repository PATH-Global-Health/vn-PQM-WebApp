﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class DimYear : BaseEntity
    {
        public int Year { get; set; }

        public virtual ICollection<DimMonth> Months { get; set; }
    }
}
