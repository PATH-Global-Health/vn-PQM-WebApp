using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class GenderCreateModel
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string CreatedBy { get; set; }
    }
    public class GenderViewModel : GenderCreateModel
    {
        public Guid Id { get; set; }
    }
}
