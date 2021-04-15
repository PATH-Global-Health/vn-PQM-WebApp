using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class CategoryAlias : BaseEntity
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Category { get; set; }
    }
}
