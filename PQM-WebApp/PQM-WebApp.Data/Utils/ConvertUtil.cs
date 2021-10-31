using Newtonsoft.Json;
using PQM_WebApp.Data.Entities;
using PQM_WebApp.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.Utils
{
    public static class ConvertUtil
    {
        public static LanguageDictionary ConvertToDictionary(List<LanguageDictionaryViewModel> dict)
        {
            LanguageDictionary dictionary = null;
            if (dict != null)
            {
                dictionary = new LanguageDictionary();
                dictionary.Dictionary = JsonConvert.SerializeObject(dict);
            }

            return dictionary;
        }
    }
}
