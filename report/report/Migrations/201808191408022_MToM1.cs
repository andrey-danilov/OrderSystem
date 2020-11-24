namespace report.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MToM1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Parameters", "ProductPars_Id", "dbo.ProductPars");
            DropForeignKey("dbo.Pictures", "ProductPars_Id", "dbo.ProductPars");
            DropIndex("dbo.Parameters", new[] { "ProductPars_Id" });
            DropIndex("dbo.Pictures", new[] { "ProductPars_Id" });
            CreateTable(
                "dbo.ProductParsParameters",
                c => new
                    {
                        ProductPars_Id = c.String(nullable: false, maxLength: 128),
                        Parameters_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.ProductPars_Id, t.Parameters_Id })
                .ForeignKey("dbo.ProductPars", t => t.ProductPars_Id, cascadeDelete: true)
                .ForeignKey("dbo.Parameters", t => t.Parameters_Id, cascadeDelete: true)
                .Index(t => t.ProductPars_Id)
                .Index(t => t.Parameters_Id);
            
            CreateTable(
                "dbo.PictureProductPars",
                c => new
                    {
                        Picture_id = c.String(nullable: false, maxLength: 128),
                        ProductPars_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Picture_id, t.ProductPars_Id })
                .ForeignKey("dbo.Pictures", t => t.Picture_id, cascadeDelete: true)
                .ForeignKey("dbo.ProductPars", t => t.ProductPars_Id, cascadeDelete: true)
                .Index(t => t.Picture_id)
                .Index(t => t.ProductPars_Id);
            
            DropColumn("dbo.Parameters", "ProductPars_Id");
            DropColumn("dbo.Pictures", "ProductPars_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pictures", "ProductPars_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.Parameters", "ProductPars_Id", c => c.String(maxLength: 128));
            DropForeignKey("dbo.PictureProductPars", "ProductPars_Id", "dbo.ProductPars");
            DropForeignKey("dbo.PictureProductPars", "Picture_id", "dbo.Pictures");
            DropForeignKey("dbo.ProductParsParameters", "Parameters_Id", "dbo.Parameters");
            DropForeignKey("dbo.ProductParsParameters", "ProductPars_Id", "dbo.ProductPars");
            DropIndex("dbo.PictureProductPars", new[] { "ProductPars_Id" });
            DropIndex("dbo.PictureProductPars", new[] { "Picture_id" });
            DropIndex("dbo.ProductParsParameters", new[] { "Parameters_Id" });
            DropIndex("dbo.ProductParsParameters", new[] { "ProductPars_Id" });
            DropTable("dbo.PictureProductPars");
            DropTable("dbo.ProductParsParameters");
            CreateIndex("dbo.Pictures", "ProductPars_Id");
            CreateIndex("dbo.Parameters", "ProductPars_Id");
            AddForeignKey("dbo.Pictures", "ProductPars_Id", "dbo.ProductPars", "Id");
            AddForeignKey("dbo.Parameters", "ProductPars_Id", "dbo.ProductPars", "Id");
        }
    }
}
