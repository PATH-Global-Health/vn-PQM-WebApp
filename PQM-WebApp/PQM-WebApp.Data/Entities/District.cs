using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class District : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public Guid CityId { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }
        public virtual ICollection<Ward> Wards { get; set; }
    }
}
