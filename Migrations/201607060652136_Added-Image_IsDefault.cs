namespace $safeprojectname$.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedImage_IsDefault : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Images", "IsDefault", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Images", "IsDefault");
        }
    }
}
