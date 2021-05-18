using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.Entities
{
    public class Language : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public LanguageDictionary Dictionary;

    }
}
