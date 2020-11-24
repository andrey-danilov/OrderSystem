namespace report.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PE : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PromExports",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Url = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PromExports", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.PromExports", new[] { "ApplicationUser_Id" });
            DropTable("dbo.PromExports");
        }
    }
}
