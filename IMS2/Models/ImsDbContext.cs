using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace IMS2.Models
{
    public partial class ImsDbContext : DbContext
    {
        public ImsDbContext()
               : base("name=Ims2Design")
        {
        }

        public virtual DbSet<DataSourceSystem> DataSourceSystems { get; set; }
        public virtual DbSet<DepartmentCategory> DepartmentCategories { get; set; }
        public virtual DbSet<DepartmentCategoryMapIndicatorGroup> DepartmentCategoryMapIndicatorGroups { get; set; }
        public virtual DbSet<DepartmentIndicatorStandard> DepartmentIndicatorStandards { get; set; }
        public virtual DbSet<DepartmentIndicatorValue> DepartmentIndicatorValues { get; set; }
        public virtual DbSet<DepartmentIndicatorDurationVirtualValue> DepartmentIndicatorDurationVirtualValues { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Duration> Durations { get; set; }
        public virtual DbSet<IndicatorAlgorithm> IndicatorAlgorithms { get; set; }
        public virtual DbSet<IndicatorGroupMapIndicator> IndicatorGroupMapIndicators { get; set; }
        public virtual DbSet<IndicatorGroup> IndicatorGroups { get; set; }
        public virtual DbSet<Indicator> Indicators { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataSourceSystem>()
                .Property(e => e.Priority)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DataSourceSystem>()
                .Property(e => e.TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<DepartmentCategory>()
                .Property(e => e.Priority)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DepartmentCategory>()
                .Property(e => e.TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<DepartmentCategory>()
                .HasMany(e => e.DepartmentCategoryMapIndicatorGroups)
                .WithRequired(e => e.DepartmentCategory)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DepartmentCategoryMapIndicatorGroup>()
                .Property(e => e.Priority)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DepartmentCategoryMapIndicatorGroup>()
                .Property(e => e.TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<DepartmentIndicatorStandard>()
                .Property(e => e.UpperBound)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DepartmentIndicatorStandard>()
                .Property(e => e.LowerBound)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DepartmentIndicatorStandard>()
                .Property(e => e.TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<DepartmentIndicatorStandard>()
                .HasMany(e => e.DepartmentIndicatorValues)
                .WithOptional(e => e.DepartmentIndicatorStandard)
                .HasForeignKey(e => e.IndicatorStandardId);

            modelBuilder.Entity<DepartmentIndicatorValue>()
                .Property(e => e.TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<Department>()
                .Property(e => e.Priority)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Department>()
                .Property(e => e.TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<Department>()
                .HasMany(e => e.DepartmentIndicatorValues)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Indicators)
                .WithOptional(e => e.Department)
                .HasForeignKey(e => e.DutyDepartmentId);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.ProvidingIndicators)
                .WithOptional(e => e.ProvidingDepartment)
                .HasForeignKey(e => e.ProvidingDepartmentId);

            modelBuilder.Entity<IndicatorGroupMapIndicator>()
                .Property(e => e.Priority)
                .HasPrecision(18, 2);

            modelBuilder.Entity<IndicatorGroupMapIndicator>()
                .Property(e => e.TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<IndicatorGroup>()
                .Property(e => e.Priority)
                .HasPrecision(18, 2);

            modelBuilder.Entity<IndicatorGroup>()
                .Property(e => e.TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<IndicatorGroup>()
                .HasMany(e => e.DepartmentCategoryMapIndicatorGroups)
                .WithRequired(e => e.IndicatorGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<IndicatorGroup>()
                .HasMany(e => e.IndicatorGroupMapIndicators)
                .WithRequired(e => e.IndicatorGroup)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Indicator>()
                .Property(e => e.Priority)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Indicator>()
                .Property(e => e.TimeStamp)
                .IsFixedLength();

            modelBuilder.Entity<Indicator>()
                .HasMany(e => e.IndicatorGroupMapIndicators)
                .WithRequired(e => e.Indicator)
                .WillCascadeOnDelete(false);

            #region 修改DepartmentIndicatorDurationVirtualValue中的Value精度
            modelBuilder.Entity<DepartmentIndicatorDurationVirtualValue>()
                .Property(e => e.Value)
                .HasPrecision(18, 4);
            #endregion
        }
    }
}