using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class Site : DimensionGroup
    {
        public string Code { get; set; }

        public Guid DistrictId { get; set; }
        [ForeignKey("DistrictId")]
        public virtual District District { get; set; }
    }
}
