namespace IMS2.ImsDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDScaffle : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DataSourceSystems",
                c => new
                    {
                        DataSourceSystemId = c.Guid(nullable: false),
                        DataSourceSystemName = c.String(nullable: false),
                        Priority = c.Decimal(nullable: false, precision: 18, scale: 0),
                        Remarks = c.String(),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.DataSourceSystemId);
            
            CreateTable(
                "dbo.Indicators",
                c => new
                    {
                        IndicatorId = c.Guid(nullable: false),
                        IndicatorName = c.String(nullable: false),
                        Unit = c.String(nullable: false),
                        IsAutoGetData = c.Boolean(nullable: false),
                        ProvidingDepartmentId = c.Guid(),
                        DataSourceSystemId = c.Guid(),
                        DutyDepartmentId = c.Guid(),
                        DurationId = c.Guid(),
                        Priority = c.Decimal(nullable: false, precision: 18, scale: 0),
                        Remarks = c.String(),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.IndicatorId)
                .ForeignKey("dbo.DataSourceSystems", t => t.DataSourceSystemId)
                .ForeignKey("dbo.Departments", t => t.DutyDepartmentId)
                .ForeignKey("dbo.Departments", t => t.ProvidingDepartmentId)
                .ForeignKey("dbo.Durations", t => t.DurationId)
                .Index(t => t.ProvidingDepartmentId)
                .Index(t => t.DataSourceSystemId)
                .Index(t => t.DutyDepartmentId)
                .Index(t => t.DurationId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentId = c.Guid(nullable: false),
                        DepartmentCategoryId = c.Guid(nullable: false),
                        DepartmentName = c.String(nullable: false),
                        Priority = c.Decimal(nullable: false, precision: 18, scale: 0),
                        Remarks = c.String(),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.DepartmentId)
                .ForeignKey("dbo.DepartmentCategories", t => t.DepartmentCategoryId, cascadeDelete: true)
                .Index(t => t.DepartmentCategoryId);
            
            CreateTable(
                "dbo.DepartmentCategories",
                c => new
                    {
                        DepartmentCategoryId = c.Guid(nullable: false),
                        DepartmentCategoryName = c.String(nullable: false, maxLength: 128),
                        Priority = c.Decimal(nullable: false, precision: 18, scale: 0),
                        Remarks = c.String(),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.DepartmentCategoryId);
            
            CreateTable(
                "dbo.DepartmentCategoryMapIndicatorGroups",
                c => new
                    {
                        DepartmentCategoryMapIndicatorGroupId = c.Guid(nullable: false),
                        DepartmentCategoryId = c.Guid(nullable: false),
                        IndicatorGroupId = c.Guid(nullable: false),
                        Priority = c.Decimal(nullable: false, precision: 18, scale: 0),
                        Remarks = c.String(),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.DepartmentCategoryMapIndicatorGroupId)
                .ForeignKey("dbo.IndicatorGroups", t => t.IndicatorGroupId)
                .ForeignKey("dbo.DepartmentCategories", t => t.DepartmentCategoryId)
                .Index(t => t.DepartmentCategoryId)
                .Index(t => t.IndicatorGroupId);
            
            CreateTable(
                "dbo.IndicatorGroups",
                c => new
                    {
                        IndicatorGroupId = c.Guid(nullable: false),
                        IndicatorGroupName = c.String(nullable: false),
                        Priority = c.Decimal(nullable: false, precision: 18, scale: 0),
                        Remarks = c.String(),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.IndicatorGroupId);
            
            CreateTable(
                "dbo.IndicatorGroupMapIndicators",
                c => new
                    {
                        IndicatorGroupMapIndicatorId = c.Guid(nullable: false),
                        IndicatorGroupId = c.Guid(nullable: false),
                        IndicatorId = c.Guid(nullable: false),
                        Priority = c.Decimal(nullable: false, precision: 18, scale: 0),
                        Remarks = c.String(),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.IndicatorGroupMapIndicatorId)
                .ForeignKey("dbo.IndicatorGroups", t => t.IndicatorGroupId)
                .ForeignKey("dbo.Indicators", t => t.IndicatorId)
                .Index(t => t.IndicatorGroupId)
                .Index(t => t.IndicatorId);
            
            CreateTable(
                "dbo.DepartmentIndicatorStandards",
                c => new
                    {
                        DepartmentIndicatorStandardId = c.Guid(nullable: false),
                        DepartmentId = c.Guid(nullable: false),
                        IndicatorId = c.Guid(nullable: false),
                        UpperBound = c.Decimal(precision: 18, scale: 0),
                        UpperBoundIncluded = c.Boolean(),
                        LowerBound = c.Decimal(precision: 18, scale: 0),
                        LowerBoundIncluded = c.Boolean(),
                        UpdateTime = c.DateTime(nullable: false),
                        Version = c.Int(nullable: false),
                        Remarks = c.String(),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.DepartmentIndicatorStandardId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.Indicators", t => t.IndicatorId, cascadeDelete: true)
                .Index(t => t.DepartmentId)
                .Index(t => t.IndicatorId);
            
            CreateTable(
                "dbo.DepartmentIndicatorValues",
                c => new
                    {
                        DepartmentIndicatorValueId = c.Guid(nullable: false),
                        DepartmentId = c.Guid(nullable: false),
                        IndicatorId = c.Guid(nullable: false),
                        Time = c.DateTime(nullable: false),
                        Value = c.Decimal(precision: 18, scale: 2),
                        IndicatorStandardId = c.Guid(),
                        IsLocked = c.Boolean(nullable: false),
                        UpdateTime = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.DepartmentIndicatorValueId)
                .ForeignKey("dbo.Indicators", t => t.IndicatorId, cascadeDelete: true)
                .ForeignKey("dbo.DepartmentIndicatorStandards", t => t.IndicatorStandardId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .Index(t => t.DepartmentId)
                .Index(t => t.IndicatorId)
                .Index(t => t.IndicatorStandardId);
            
            CreateTable(
                "dbo.Durations",
                c => new
                    {
                        DurationId = c.Guid(nullable: false),
                        DurationName = c.String(nullable: false, maxLength: 50),
                        Remarks = c.String(),
                    })
                .PrimaryKey(t => t.DurationId);
            
            CreateTable(
                "dbo.IndicatorAlgorithms",
                c => new
                    {
                        IndicatorAlgorithmsId = c.Guid(nullable: false),
                        ResultId = c.Guid(nullable: false),
                        FirstOperandID = c.Guid(nullable: false),
                        SecondOperandID = c.Guid(nullable: false),
                        OperationMethod = c.String(nullable: false),
                        Remarks = c.String(),
                    })
                .PrimaryKey(t => t.IndicatorAlgorithmsId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.IndicatorGroupMapIndicators", "IndicatorId", "dbo.Indicators");
            DropForeignKey("dbo.Indicators", "DurationId", "dbo.Durations");
            DropForeignKey("dbo.Indicators", "ProvidingDepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Indicators", "DutyDepartmentId", "dbo.Departments");
            DropForeignKey("dbo.DepartmentIndicatorValues", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.DepartmentIndicatorStandards", "IndicatorId", "dbo.Indicators");
            DropForeignKey("dbo.DepartmentIndicatorValues", "IndicatorStandardId", "dbo.DepartmentIndicatorStandards");
            DropForeignKey("dbo.DepartmentIndicatorValues", "IndicatorId", "dbo.Indicators");
            DropForeignKey("dbo.DepartmentIndicatorStandards", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.Departments", "DepartmentCategoryId", "dbo.DepartmentCategories");
            DropForeignKey("dbo.DepartmentCategoryMapIndicatorGroups", "DepartmentCategoryId", "dbo.DepartmentCategories");
            DropForeignKey("dbo.IndicatorGroupMapIndicators", "IndicatorGroupId", "dbo.IndicatorGroups");
            DropForeignKey("dbo.DepartmentCategoryMapIndicatorGroups", "IndicatorGroupId", "dbo.IndicatorGroups");
            DropForeignKey("dbo.Indicators", "DataSourceSystemId", "dbo.DataSourceSystems");
            DropIndex("dbo.DepartmentIndicatorValues", new[] { "IndicatorStandardId" });
            DropIndex("dbo.DepartmentIndicatorValues", new[] { "IndicatorId" });
            DropIndex("dbo.DepartmentIndicatorValues", new[] { "DepartmentId" });
            DropIndex("dbo.DepartmentIndicatorStandards", new[] { "IndicatorId" });
            DropIndex("dbo.DepartmentIndicatorStandards", new[] { "DepartmentId" });
            DropIndex("dbo.IndicatorGroupMapIndicators", new[] { "IndicatorId" });
            DropIndex("dbo.IndicatorGroupMapIndicators", new[] { "IndicatorGroupId" });
            DropIndex("dbo.DepartmentCategoryMapIndicatorGroups", new[] { "IndicatorGroupId" });
            DropIndex("dbo.DepartmentCategoryMapIndicatorGroups", new[] { "DepartmentCategoryId" });
            DropIndex("dbo.Departments", new[] { "DepartmentCategoryId" });
            DropIndex("dbo.Indicators", new[] { "DurationId" });
            DropIndex("dbo.Indicators", new[] { "DutyDepartmentId" });
            DropIndex("dbo.Indicators", new[] { "DataSourceSystemId" });
            DropIndex("dbo.Indicators", new[] { "ProvidingDepartmentId" });
            DropTable("dbo.IndicatorAlgorithms");
            DropTable("dbo.Durations");
            DropTable("dbo.DepartmentIndicatorValues");
            DropTable("dbo.DepartmentIndicatorStandards");
            DropTable("dbo.IndicatorGroupMapIndicators");
            DropTable("dbo.IndicatorGroups");
            DropTable("dbo.DepartmentCategoryMapIndicatorGroups");
            DropTable("dbo.DepartmentCategories");
            DropTable("dbo.Departments");
            DropTable("dbo.Indicators");
            DropTable("dbo.DataSourceSystems");
        }
    }
}
