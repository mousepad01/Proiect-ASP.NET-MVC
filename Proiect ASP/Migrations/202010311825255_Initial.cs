namespace Proiect_ASP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categorii",
                c => new
                    {
                        idCategorie = c.Int(nullable: false, identity: true),
                        titlu = c.String(nullable: false),
                        descriere = c.String(),
                    })
                .PrimaryKey(t => t.idCategorie);
            
            CreateTable(
                "dbo.CategoriiProduse",
                c => new
                    {
                        idProdusCategorie = c.Int(nullable: false, identity: true),
                        idProdus = c.Int(nullable: false),
                        idCategorie = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idProdusCategorie)
                .ForeignKey("dbo.Categorii", t => t.idCategorie, cascadeDelete: true)
                .ForeignKey("dbo.Produse", t => t.idProdus, cascadeDelete: true)
                .Index(t => t.idProdus)
                .Index(t => t.idCategorie);
            
            CreateTable(
                "dbo.Produse",
                c => new
                    {
                        idProdus = c.Int(nullable: false, identity: true),
                        idOowner = c.Int(nullable: false),
                        titlu = c.String(nullable: false),
                        descriere = c.String(),
                        imagePath = c.String(),
                        pret = c.Int(nullable: false),
                        dataAdaugare = c.DateTime(nullable: false),
                        cantitate = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.idProdus);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CategoriiProduse", "idProdus", "dbo.Produse");
            DropForeignKey("dbo.CategoriiProduse", "idCategorie", "dbo.Categorii");
            DropIndex("dbo.CategoriiProduse", new[] { "idCategorie" });
            DropIndex("dbo.CategoriiProduse", new[] { "idProdus" });
            DropTable("dbo.Produse");
            DropTable("dbo.CategoriiProduse");
            DropTable("dbo.Categorii");
        }
    }
}
