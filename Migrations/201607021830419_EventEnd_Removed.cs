namespace $safeprojectname$.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventEnd_Removed : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Events", "EventEnd");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "EventEnd", c => c.DateTime(nullable: false));
        }
    }
}
