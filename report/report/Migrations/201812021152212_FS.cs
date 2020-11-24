namespace report.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FS : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProms", "FromShop", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProms", "FromShop");
        }
    }
}
