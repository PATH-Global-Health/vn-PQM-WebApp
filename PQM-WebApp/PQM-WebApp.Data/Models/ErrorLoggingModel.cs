using System;
namespace PQM_WebApp.Data.Models
{
    public class ErrorDetailLogging
    {
        public int Row { get; set; }
        public string Indicator { get; set; }
        public string Site { get; set; }
        public string Province { get; set; }
        public string Code { get; set; }
        public string Error { get; set; }
        public object raw_data { get; set; }
    }

    public class ErrorLoggingModel
    {
        public Guid Id { get; set; }
        public ResultModel Result { get; set; }
        public object RawData { get; set; }
        public DateTime DateTime { get; set; }
        public ErrorDetailLogging Detail { get; set; }
    }
}
