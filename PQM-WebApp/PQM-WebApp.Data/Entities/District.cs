using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class District : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Slug { get; set; }
        public string NameWithType { get; set; }
        public string Path { get; set; }
        public string PathWithType { get; set; }
        public string ParentCode { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }

        public Guid ProvinceId { get; set; }
        [ForeignKey("ProvinceId")]
        public virtual Province Province { get; set; }

        public virtual ICollection<Site> Sites { get; set; }
    }
}
