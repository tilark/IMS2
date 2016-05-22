namespace IMS2.ImsDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DecimalTo2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DataSourceSystems", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Indicators", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Departments", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.DepartmentCategories", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.DepartmentCategoryMapIndicatorGroups", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.IndicatorGroups", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.IndicatorGroupMapIndicators", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.DepartmentIndicatorStandards", "UpperBound", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.DepartmentIndicatorStandards", "LowerBound", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DepartmentIndicatorStandards", "LowerBound", c => c.Decimal(precision: 18, scale: 0));
            AlterColumn("dbo.DepartmentIndicatorStandards", "UpperBound", c => c.Decimal(precision: 18, scale: 0));
            AlterColumn("dbo.IndicatorGroupMapIndicators", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 0));
            AlterColumn("dbo.IndicatorGroups", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 0));
            AlterColumn("dbo.DepartmentCategoryMapIndicatorGroups", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 0));
            AlterColumn("dbo.DepartmentCategories", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 0));
            AlterColumn("dbo.Departments", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 0));
            AlterColumn("dbo.Indicators", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 0));
            AlterColumn("dbo.DataSourceSystems", "Priority", c => c.Decimal(nullable: false, precision: 18, scale: 0));
        }
    }
}
