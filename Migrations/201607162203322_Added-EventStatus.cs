namespace $safeprojectname$.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEventStatus : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Events", new[] { "Open" });
            AddColumn("dbo.Events", "Status", c => c.String(nullable: false, maxLength: 20, unicode: false));
            CreateIndex("dbo.Events", "Status");
            DropColumn("dbo.Events", "Open");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "Open", c => c.Boolean(nullable: false));
            DropIndex("dbo.Events", new[] { "Status" });
            DropColumn("dbo.Events", "Status");
            CreateIndex("dbo.Events", "Open");
        }
    }
}
