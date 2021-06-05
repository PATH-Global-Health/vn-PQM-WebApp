using System;
namespace PQM_WebApp.Data.Entities
{
    public enum DataPermissionType
    {
        Read,
        Write,
    }

    public class DataPermission : BaseEntity
    {
        public string Username { get; set; }
        public Guid ProvinceId { get; set; }
        public Guid? IndicatorId { get; set; }
        public DataPermissionType Type {get;set;}
    }
}
