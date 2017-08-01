namespace IMS2.ImsDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewDepartmentIndicatorVirtualValueTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DepartmentIndicatorDurationVirtualValues",
                c => new
                    {
                        DepartmentIndicatorDurationVirtualValueID = c.Guid(nullable: false),
                        DepartmentId = c.Guid(nullable: false),
                        IndicatorId = c.Guid(nullable: false),
                        DurationId = c.Guid(nullable: false),
                        Time = c.DateTime(nullable: false),
                        Value = c.Decimal(precision: 18, scale: 2),
                        CreateTime = c.DateTime(nullable: false),
                        UpdateTime = c.DateTime(nullable: false),
                        TimeStamp = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.DepartmentIndicatorDurationVirtualValueID)
                .ForeignKey("dbo.Departments", t => t.DepartmentId, cascadeDelete: true)
                .ForeignKey("dbo.Durations", t => t.DurationId, cascadeDelete: true)
                .ForeignKey("dbo.Indicators", t => t.IndicatorId, cascadeDelete: true)
                .Index(t => t.DepartmentId)
                .Index(t => t.IndicatorId)
                .Index(t => t.DurationId);
            
            AddColumn("dbo.Durations", "Level", c => c.Int());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DepartmentIndicatorDurationVirtualValues", "IndicatorId", "dbo.Indicators");
            DropForeignKey("dbo.DepartmentIndicatorDurationVirtualValues", "DurationId", "dbo.Durations");
            DropForeignKey("dbo.DepartmentIndicatorDurationVirtualValues", "DepartmentId", "dbo.Departments");
            DropIndex("dbo.DepartmentIndicatorDurationVirtualValues", new[] { "DurationId" });
            DropIndex("dbo.DepartmentIndicatorDurationVirtualValues", new[] { "IndicatorId" });
            DropIndex("dbo.DepartmentIndicatorDurationVirtualValues", new[] { "DepartmentId" });
            DropColumn("dbo.Durations", "Level");
            DropTable("dbo.DepartmentIndicatorDurationVirtualValues");
        }
    }
}
