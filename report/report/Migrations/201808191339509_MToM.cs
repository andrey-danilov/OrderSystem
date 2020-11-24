namespace report.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MToM : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parameters", "IdProd", c => c.String());
            AddColumn("dbo.Pictures", "IdProd", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Pictures", "IdProd");
            DropColumn("dbo.Parameters", "IdProd");
        }
    }
}
