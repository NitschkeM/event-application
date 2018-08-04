namespace $safeprojectname$.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageClass_Added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ImageUrl = c.String(),
                        SizeInBytes = c.Long(nullable: false),
                        SizeInKb = c.Long(nullable: false),
                        UserId = c.String(maxLength: 128),
                        EventId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.Events", t => t.EventId)
                .Index(t => t.UserId)
                .Index(t => t.EventId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Images", "EventId", "dbo.Events");
            DropForeignKey("dbo.Images", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Images", new[] { "EventId" });
            DropIndex("dbo.Images", new[] { "UserId" });
            DropTable("dbo.Images");
        }
    }
}
