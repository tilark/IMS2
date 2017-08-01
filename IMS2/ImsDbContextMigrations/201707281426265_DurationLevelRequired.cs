namespace IMS2.ImsDbContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DurationLevelRequired : DbMigration
    {
        public override void Up()
        {
            Sql("UPDATE [dbo].[Durations]   SET  [Level] = 1");
            AlterColumn("dbo.Durations", "Level", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Durations", "Level", c => c.Int());
        }
    }
}
