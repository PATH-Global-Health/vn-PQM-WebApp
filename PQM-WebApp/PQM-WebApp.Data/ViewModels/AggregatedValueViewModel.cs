using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class AggregatedValueViewModel
    {
        public Guid Id { get; set; }
        public DataType DataType { get; set; }
        public int Denominator { get; set; }
        public int Numerator { get; set; }
        //Time Dimension
        public string PeriodType { get; set; } //Day, Month, Quarter, Year
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
        //Location Dimension
        public SiteViewModel Site { get; set; }
        public IndicatorViewModel Indicator { get; set; }
        //Gender Dimension
        public GenderViewModel Gender { get; set; }
        //Age Group Dimension
        public AgeGroupViewModel AgeGroup { get; set; }
        //validation
        public bool IsValid { get; set; }
        public string InvalidMessage { get; set; }
        //Key population Dimension
        public KeyPopulationViewModel KeyPopulation { get; set; }
        public string DrugName { get; set; }
        public string DrugUnitName { get; set; }
        public string DataSource { get; set; }
        public virtual ICollection<UnsolvedDimValueViewModel> UnsolvedDimValues { get; set; }
    }

    public class AggregatedValueUpdaetModel
    {
        public Guid Id { get; set; }
        public int Numerator { get; set; }
        public int? Denominator { get; set; }
    }
}
