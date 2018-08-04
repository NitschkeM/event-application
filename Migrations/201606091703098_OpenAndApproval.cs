namespace $safeprojectname$.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OpenAndApproval : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "Open", c => c.Boolean(nullable: false));
            AddColumn("dbo.Events", "ApprovalReq", c => c.Boolean(nullable: false));
            CreateIndex("dbo.Events", "Open");
            CreateIndex("dbo.Events", "ApprovalReq");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Events", new[] { "ApprovalReq" });
            DropIndex("dbo.Events", new[] { "Open" });
            DropColumn("dbo.Events", "ApprovalReq");
            DropColumn("dbo.Events", "Open");
        }
    }
}
