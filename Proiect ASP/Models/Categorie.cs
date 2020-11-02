using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Proiect_ASP.Models
{
    [Table("Categorii")]
    public class Categorie
    {
        [Key]
        public int idCategorie { get; set; }

        [Required]
        public string titlu { get; set; }
        public string descriere { get; set; }

        public virtual ICollection<CategorieProdus> CategoriiProduse { get; set; }
    }
}