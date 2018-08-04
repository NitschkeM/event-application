namespace $safeprojectname$.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTags : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Count = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Count);
            
            CreateTable(
                "dbo.TagEvents",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Event_EventId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Event_EventId })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.Events", t => t.Event_EventId, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Event_EventId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TagEvents", "Event_EventId", "dbo.Events");
            DropForeignKey("dbo.TagEvents", "Tag_Id", "dbo.Tags");
            DropIndex("dbo.TagEvents", new[] { "Event_EventId" });
            DropIndex("dbo.TagEvents", new[] { "Tag_Id" });
            DropIndex("dbo.Tags", new[] { "Count" });
            DropTable("dbo.TagEvents");
            DropTable("dbo.Tags");
        }
    }
}
