namespace $safeprojectname$.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class cascadeDelete_UserToCreatedEvent : DbMigration
    {
        public override void Up()
        {
            // Introducing FOREIGN KEY constraint 'FK_dbo.Events_dbo.AspNetUsers_CreatorId' on table 'Events' may cause cycles or multiple cascade paths. 
            // Specify ON DELETE NO ACTION or ON UPDATE NO ACTION, or modify other FOREIGN KEY constraints.
            DropForeignKey("dbo.Events", "CreatorId", "dbo.AspNetUsers");
            AddForeignKey("dbo.Events", "CreatorId", "dbo.AspNetUsers", "Id", false);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Events", "CreatorId", "dbo.AspNetUsers");
            AddForeignKey("dbo.Events", "CreatorId", "dbo.AspNetUsers", "Id", false);
        }
    }
}
