namespace report.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SN : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderProms", "ShopNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderProms", "ShopNumber");
        }
    }
}
