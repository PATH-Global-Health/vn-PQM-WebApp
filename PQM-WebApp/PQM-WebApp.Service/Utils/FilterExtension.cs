using PQM_WebApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PQM_WebApp.Service.Utils
{
    public static class FilterExtension
    {
        public static IQueryable<T> AsSoftDelete<T>(this IQueryable<T> data, bool includeDeletedItem) where T : BaseEntity
        {
            return data.Where(s => includeDeletedItem ? true : !s.IsDeleted);
        }
    }
}
