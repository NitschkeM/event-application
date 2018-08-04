namespace $safeprojectname$.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShortDesc_Removed : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Events", "ShortDescription");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "ShortDescription", c => c.String(nullable: false));
        }
    }
}
