namespace $safeprojectname$.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedEventStatus_Name : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Events", new[] { "Status" });
            AddColumn("dbo.Events", "EventStatus", c => c.String(nullable: false, maxLength: 20, unicode: false));
            CreateIndex("dbo.Events", "EventStatus");
            DropColumn("dbo.Events", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "Status", c => c.String(nullable: false, maxLength: 20, unicode: false));
            DropIndex("dbo.Events", new[] { "EventStatus" });
            DropColumn("dbo.Events", "EventStatus");
            CreateIndex("dbo.Events", "Status");
        }
    }
}
