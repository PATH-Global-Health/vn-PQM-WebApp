using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class AgeGroupCreateModel
    {
        public string Name { get; set; }
        public byte? From { get; set; }
        public byte? To { get; set; }
        public int Order { get; set; }
    }

    public class AgeGroupViewModel : AgeGroupCreateModel
    {
        public Guid Id { get; set; }
    }
}
