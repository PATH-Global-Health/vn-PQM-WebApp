using System;
using PQM_WebApp.Data.Entities;

namespace PQM_WebApp.Data.ViewModels
{
    public class DataPermissionViewModel : DataPermissionCreateModel
    {
        public Guid Id { get; set; }
    }

    public class DataPermissionCreateModel
    {
        public string Username { get; set; }
        public Guid ProvinceId { get; set; }
        public Guid? IndicatorId { get; set; }
        public DataPermissionType Type { get; set; }
    }
}
