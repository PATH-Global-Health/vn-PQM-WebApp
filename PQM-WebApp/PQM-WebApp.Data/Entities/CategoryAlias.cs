using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class CategoryAlias : BaseEntity
    {
        public Guid CategoryId { get; set; }
        public string Alias { get; set; }
        public string Category { get; set; }
    }
}
