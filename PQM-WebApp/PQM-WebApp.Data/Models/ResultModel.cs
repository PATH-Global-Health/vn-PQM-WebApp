using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.Models
{
    public class ErrorModel
    {
        public string Code { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ResultModel
    {
        public ResultModel()
        {
            Error = new ErrorModel();
        }
        public ErrorModel Error { get; set; }
        public object Data { get; set; }
        public bool Succeed { get; set; }

    }

    public class PagingModel : ResultModel
    {
        public int PageCount { get; set; }
        public int Total { get; set; }
    }
}
