using System;
namespace PQM_WebApp.Service.Utils
{
    public static class DateExtension
    {
        public static int Quarter(this DateTime date)
        {
            var month = date.Month;
            if (month <= 3) return 1;
            if (month <= 6) return 2;
            if (month <= 9) return 3;
            return 4;
        }
    }
}
