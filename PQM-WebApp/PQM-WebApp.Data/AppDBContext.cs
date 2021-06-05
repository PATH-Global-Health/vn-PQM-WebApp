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
        public DbSet<District> Districts { get; set; }
        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<IndicatorGroup> IndicatorGroups { get; set; }
        public DbSet<KeyPopulation> KeyPopulations { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Gender> Gender { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<ThresholdSetting> ThresholdSettings { get; set; }
        public DbSet<SiteType> SiteTypes { get; set; }
        public DbSet<CategoryAlias> CategoryAliases { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<LanguageDictionary> LanguageDictionaries { get; set; }
        public DbSet<UndefinedDimValue> UndefinedDimValues { get; set; }
        public DbSet<UnsolvedDimValue> UnsolvedDimValues { get; set; }
        public DbSet<DataPermission> DataPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            };
            modelBuilder.Entity<UnsolvedDimValue>().HasKey(sc => new { sc.AggregatedValueId, sc.UndefinedDimValueId });
        }
    }

    // For Package Console
    // Add-Migration PQM_DB_v1
    // Update-Database
    //
    // For Terminal
    // dotnet ef migrations add PQM_DB_v3_3 -s PQM-WebApp -p PQM-WebApp.Data
    // dotnet ef database update -s PQM-WebApp -p PQM-WebApp.Data
}
