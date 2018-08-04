namespace $safeprojectname$.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                {
                    CommentId = c.Int(nullable: false, identity: true),
                    CommentText = c.String(nullable: false),
                    PosterId = c.String(maxLength: 128),
                    EventId = c.Int(nullable: false),
                    PostedTime = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.AspNetUsers", t => t.PosterId)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .Index(t => t.PosterId)
                .Index(t => t.EventId);

            CreateTable(
                "dbo.AspNetUsers",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    DateOfBirth = c.DateTime(nullable: false),
                    Gender = c.String(),
                    Email = c.String(maxLength: 256),
                    EmailConfirmed = c.Boolean(nullable: false),
                    PasswordHash = c.String(),
                    SecurityStamp = c.String(),
                    PhoneNumber = c.String(),
                    PhoneNumberConfirmed = c.Boolean(nullable: false),
                    TwoFactorEnabled = c.Boolean(nullable: false),
                    LockoutEndDateUtc = c.DateTime(),
                    LockoutEnabled = c.Boolean(nullable: false),
                    AccessFailedCount = c.Int(nullable: false),
                    UserName = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");

            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    UserId = c.String(nullable: false, maxLength: 128),
                    ClaimType = c.String(),
                    ClaimValue = c.String(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Events",
                c => new
                {
                    EventId = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false),
                    Gender = c.String(nullable: false),
                    ShortDescription = c.String(nullable: false),
                    Description = c.String(nullable: false),
                    EventStart = c.DateTime(nullable: false),
                    EventEnd = c.DateTime(nullable: false),
                    AgeMin = c.Int(nullable: false),
                    AgeMax = c.Int(nullable: false),
                    PartMin = c.Int(nullable: false),
                    PartMax = c.Int(nullable: false),
                    PosLat = c.Double(nullable: false),
                    PosLng = c.Double(nullable: false),
                    CreatorId = c.String(maxLength: 128),
                })
                .PrimaryKey(t => t.EventId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatorId)
                .Index(t => t.CreatorId);

            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                {
                    LoginProvider = c.String(nullable: false, maxLength: 128),
                    ProviderKey = c.String(nullable: false, maxLength: 128),
                    UserId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128),
                    RoleId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.AspNetRoles",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 128),
                    Name = c.String(nullable: false, maxLength: 256),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "dbo.ParticipantEvent",
                c => new
                {
                    EventId = c.Int(nullable: false),
                    ParticipantId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.EventId, t.ParticipantId })
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ParticipantId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.ParticipantId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Events", "CreatorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ParticipantEvent", "ParticipantId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ParticipantEvent", "EventId", "dbo.Events");
            DropForeignKey("dbo.Comments", "EventId", "dbo.Events");
            DropForeignKey("dbo.Comments", "PosterId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.ParticipantEvent", new[] { "ParticipantId" });
            DropIndex("dbo.ParticipantEvent", new[] { "EventId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Events", new[] { "CreatorId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Comments", new[] { "EventId" });
            DropIndex("dbo.Comments", new[] { "PosterId" });
            DropTable("dbo.ParticipantEvent");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Events");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Comments");
        }
    }
}
