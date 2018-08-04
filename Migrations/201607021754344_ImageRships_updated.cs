namespace $safeprojectname$.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImageRships_updated : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Images", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Images", "EventId", "dbo.Events");
            DropIndex("dbo.Images", new[] { "UserId" });
            DropIndex("dbo.Images", new[] { "EventId" });
            AddColumn("dbo.AspNetUsers", "AboutMe", c => c.String());
            AddColumn("dbo.AspNetUsers", "PictureId", c => c.Int());
            AddColumn("dbo.Events", "PictureId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "PictureId");
            CreateIndex("dbo.Events", "PictureId");
            AddForeignKey("dbo.Events", "PictureId", "dbo.Images", "Id");
            AddForeignKey("dbo.AspNetUsers", "PictureId", "dbo.Images", "Id");
            DropColumn("dbo.Images", "UserId");
            DropColumn("dbo.Images", "EventId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Images", "EventId", c => c.Int());
            AddColumn("dbo.Images", "UserId", c => c.String(maxLength: 128));
            DropForeignKey("dbo.AspNetUsers", "PictureId", "dbo.Images");
            DropForeignKey("dbo.Events", "PictureId", "dbo.Images");
            DropIndex("dbo.Events", new[] { "PictureId" });
            DropIndex("dbo.AspNetUsers", new[] { "PictureId" });
            DropColumn("dbo.Events", "PictureId");
            DropColumn("dbo.AspNetUsers", "PictureId");
            DropColumn("dbo.AspNetUsers", "AboutMe");
            CreateIndex("dbo.Images", "EventId");
            CreateIndex("dbo.Images", "UserId");
            AddForeignKey("dbo.Images", "EventId", "dbo.Events", "EventId");
            AddForeignKey("dbo.Images", "UserId", "dbo.AspNetUsers", "Id");
        }
    }
}
