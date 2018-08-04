namespace $safeprojectname$.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PendingAndReplies : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Comments", new[] { "PosterId" });
            DropIndex("dbo.Comments", new[] { "EventId" });
            RenameColumn(table: "dbo.ParticipantEvent", name: "ParticipantId", newName: "UserId");
            RenameIndex(table: "dbo.ParticipantEvent", name: "IX_ParticipantId", newName: "IX_UserId");
            CreateTable(
                "dbo.PendingParticipants",
                c => new
                    {
                        EventId = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.EventId, t.UserId })
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.UserId);
            
            AddColumn("dbo.Comments", "ParentId", c => c.Int());
            CreateIndex("dbo.Comments", "PostedTime");
            CreateIndex("dbo.Comments", "PosterId");
            CreateIndex("dbo.Comments", "EventId");
            CreateIndex("dbo.Comments", "ParentId");
            CreateIndex("dbo.AspNetUsers", "DateOfBirth");
            CreateIndex("dbo.Events", "EventStart");
            CreateIndex("dbo.Events", "AgeMin");
            CreateIndex("dbo.Events", "AgeMax");
            CreateIndex("dbo.Events", "PartMin");
            CreateIndex("dbo.Events", "PartMax");
            AddForeignKey("dbo.Comments", "ParentId", "dbo.Comments", "CommentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "ParentId", "dbo.Comments");
            DropForeignKey("dbo.PendingParticipants", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PendingParticipants", "EventId", "dbo.Events");
            DropIndex("dbo.PendingParticipants", new[] { "UserId" });
            DropIndex("dbo.PendingParticipants", new[] { "EventId" });
            DropIndex("dbo.Events", new[] { "PartMax" });
            DropIndex("dbo.Events", new[] { "PartMin" });
            DropIndex("dbo.Events", new[] { "AgeMax" });
            DropIndex("dbo.Events", new[] { "AgeMin" });
            DropIndex("dbo.Events", new[] { "EventStart" });
            DropIndex("dbo.AspNetUsers", new[] { "DateOfBirth" });
            DropIndex("dbo.Comments", new[] { "ParentId" });
            DropIndex("dbo.Comments", new[] { "EventId" });
            DropIndex("dbo.Comments", new[] { "PosterId" });
            DropIndex("dbo.Comments", new[] { "PostedTime" });
            DropColumn("dbo.Comments", "ParentId");
            DropTable("dbo.PendingParticipants");
            RenameIndex(table: "dbo.ParticipantEvent", name: "IX_UserId", newName: "IX_ParticipantId");
            RenameColumn(table: "dbo.ParticipantEvent", name: "UserId", newName: "ParticipantId");
            CreateIndex("dbo.Comments", "EventId");
            CreateIndex("dbo.Comments", "PosterId");
        }
    }
}
