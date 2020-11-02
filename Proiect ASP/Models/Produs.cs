using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Proiect_ASP.Models
{
    [Table("Produse")]
    public class Produs
    {
        [Key]
        public int idProdus { get; set; }

        [Required]
        public int idOwner { get; set; }
        [Required]
        public string titlu { get; set; }
        public string descriere { get; set; }
        public string imagePath { get; set; }
        [Required]
        public int pret { get; set; }
        public DateTime dataAdaugare { get; set; }
        [Required]
        public int cantitate { get; set; }

        public virtual ICollection<CategorieProdus> CategoriiProduse { get; set; }
    }
}