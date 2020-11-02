using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Proiect_ASP.Models
{
    [Table("CategoriiProduse")]
    public class CategorieProdus
    {
        [Key]
        public int idProdusCategorie { get; set; }

        [Required]
        public int idProdus { get; set; }
        [Required]
        public int idCategorie { get; set; }

        public virtual Produs produs { get; set; }
        public virtual Categorie categorie { get; set; }
    }
}