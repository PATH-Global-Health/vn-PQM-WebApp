using Microsoft.EntityFrameworkCore;
using PQM_WebApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PQM_WebApp.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
        : base(options)
        { }

        public DbSet<AgeGroup> AgeGroups { get; set; }
        public DbSet<AggregatedValue> AggregatedValues { get; set; }
        public DbSet<DimDate> DimDates { get; set; }
        public DbSet<DimMonth> DimMonths { get; set; }
        public DbSet<DimYear> DimYears { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<IndicatorGroup> IndicatorGroups { get; set; }
        public DbSet<KeyPopulation> KeyPopulations { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Sex> Sex { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<SiteType> SiteTypes { get; set; }
        public DbSet<CategoryAlias> CategoryAliases { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<LanguageDictionary> LanguageDictionaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            };
        }
    }

    // Add-Migration PQM_DB_v1
    // Update-Database
}
