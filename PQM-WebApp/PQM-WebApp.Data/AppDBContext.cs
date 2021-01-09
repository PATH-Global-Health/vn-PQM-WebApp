using Microsoft.EntityFrameworkCore;
using PQM_WebApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PQM_WebApp.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
        : base(options)
        { }

        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<IndicatorGroup> IndicatorGroups { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<DimYear> DimYears { get; set; }
        public DbSet<DimQuarter> DimQuarters { get; set; }
        public DbSet<DimMonth> DimMonths { get; set; }
        public DbSet<DimWeek> DimWeeks { get; set; }
        public DbSet<DimDate> DimDates { get; set; }
        public DbSet<IndicatorSummaryValue> IndicatorSummaryValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }

}
