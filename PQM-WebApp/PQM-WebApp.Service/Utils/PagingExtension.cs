using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace PQM_WebApp.Service.Utils
{
    public static class PagingExtension
    {
        public static int PageCount<T>(this IQueryable<T> data, int pageSize)
        {
            return (int)Math.Ceiling(data.Count() / (double)pageSize);
        }

        public static IEnumerable<T> PageData<T>(this IQueryable<T> data, int pageIndex, int pageSize) where T : class
        {
            return data.Skip(pageIndex * pageSize).Take(pageSize).AsNoTracking().AsEnumerable();
        }
    }
}
