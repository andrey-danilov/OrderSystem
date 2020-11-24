namespace report.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tracking : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProms", "Traking", c => c.String());
            AddColumn("dbo.OrderProms", "TrakingStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProms", "TrakingStatus");
            DropColumn("dbo.OrderProms", "Traking");
        }
    }
}
