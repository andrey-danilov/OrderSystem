namespace report.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class xml : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Konfigs", "InQueue", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Konfigs", "InQueue");
        }
    }
}
