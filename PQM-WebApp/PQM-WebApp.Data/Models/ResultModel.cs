using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.Models
{
    public class ResultModel
    {
        public string ErrorMessage { get; set; }
        public object Data { get; set; }
        public bool Succeed { get; set; }

    }

    public class PagingModel : ResultModel
    {
        public int PageCount { get; set; }
    }
}
