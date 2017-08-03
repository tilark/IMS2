namespace IMS2.ImsDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeVirtualValuePrecisionTo4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DepartmentIndicatorDurationVirtualValues", "Value", c => c.Decimal(precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DepartmentIndicatorDurationVirtualValues", "Value", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
