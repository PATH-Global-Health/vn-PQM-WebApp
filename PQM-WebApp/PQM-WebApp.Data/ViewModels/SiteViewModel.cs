using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data.ViewModels
{
    public class SiteCreateModel
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string Code { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public Guid? DistrictId { get; set; }
        public Guid? SiteTypeId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string CreatedBy { get; set; }

        public string Address { get; set; }
        public string DistrictCode { get; set; }
    }

    public class SiteViewModel : SiteCreateModel
    {
        public Guid Id { get; set; }
        public SiteTypeViewModel SiteType { get; set; }
        public DistrictModel District { get; set; }
    }

    public class MapsResponse
    {
        public class Rootobject
        {
            public Result[] results { get; set; }
            public string status { get; set; }
        }

        public class Result
        {
            public Address_Components[] address_components { get; set; }
            public string formatted_address { get; set; }
            public Geometry geometry { get; set; }
            public string place_id { get; set; }
            public string[] types { get; set; }
        }

        public class Geometry
        {
            public Location location { get; set; }
            public string location_type { get; set; }
            public Viewport viewport { get; set; }
        }

        public class Location
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Viewport
        {
            public Northeast northeast { get; set; }
            public Southwest southwest { get; set; }
        }

        public class Northeast
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Southwest
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Address_Components
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public string[] types { get; set; }
        }

    }
}
