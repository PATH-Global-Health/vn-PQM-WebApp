using System;
namespace PQM_WebApp.Data.ViewModels
{
    public class DimensionViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Value { get; set; }
        public int? Numerator { get; set; }
        public int? Denominator { get; set; }
        public int Order { get; set; }
    }
}
