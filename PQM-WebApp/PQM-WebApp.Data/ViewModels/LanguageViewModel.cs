using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class LanguageCreateModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CreatedBy { get; set; }

        public List<LanguageDictionaryViewModel> Dictionary { get; set; }
    }

    public class LanguageViewModel : LanguageCreateModel
    {
        public Guid Id { get; set; }
    }
}
