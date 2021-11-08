using System;
namespace PQM_WebApp.Data.Models
{


    public class ErrorLoggingModel
    {
        public Guid Id { get; set; }
        public ResultModel Result { get; set; }
        public object RawData { get; set; }
        public DateTime DateTime { get; set; }
    }
}
