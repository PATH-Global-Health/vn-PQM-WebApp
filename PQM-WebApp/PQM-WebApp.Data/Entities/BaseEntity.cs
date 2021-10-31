using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public abstract partial class BaseEntity
    {
        protected BaseEntity()
        {
            IsDeleted = false;
            DateCreated = DateTime.UtcNow;
        }

        [Key, Column(Order = 0)]
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string CreatedBy { get; set; }
    }

}
