using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Proiect_ASP.Models
{
    public class AppContext : DbContext
    {
        public AppContext() : base("DBConnectionString")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppContext,
            Proiect_ASP.Migrations.Configuration>("DBConnectionString"));
        }

        public DbSet<Categorie> Categorii { get; set; }
        public DbSet<Produs> Produse { get; set; }
        public DbSet<CategorieProdus> CategoriiProduse { get; set; }
        public DbSet<ProdusRating> ProduseRatinguri { get; set; }
    }
}