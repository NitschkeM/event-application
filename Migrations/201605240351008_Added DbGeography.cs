namespace $safeprojectname$.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;
    
    public partial class AddedDbGeography : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "Coordinates", c => c.Geography(nullable: false));
            DropColumn("dbo.Events", "PosLat");
            DropColumn("dbo.Events", "PosLng");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "PosLng", c => c.Double(nullable: false));
            AddColumn("dbo.Events", "PosLat", c => c.Double(nullable: false));
            DropColumn("dbo.Events", "Coordinates");
        }
    }
}
