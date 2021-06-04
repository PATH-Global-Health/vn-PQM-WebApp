using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class DimensionGroup : BaseEntity
    {
        public string Name { get; set; }
        public int Order { get; set; }
    }

    public class Dimension
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsDeleted { get; set; }
    }
}
