using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class CategoryAliasCreateModel
    {
        protected CategoryAliasCreateModel()
        {
            DateCreated = DateTime.Now;
        }

        public Guid CategoryId { get; set; }
        public string Alias { get; set; }
        public string Category { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string CreatedBy { get; set; }
    }
    public class CategoryAliasViewModel : CategoryAliasCreateModel
    {
        public Guid Id { get; set; }
    }
}
