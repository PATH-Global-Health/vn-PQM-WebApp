using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class KeyPopulationCreateModel
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string CreatedBy { get; set; }
    }
    public class KeyPopulationViewModel : KeyPopulationCreateModel
    {
        public Guid Id { get; set; }
    }
}
