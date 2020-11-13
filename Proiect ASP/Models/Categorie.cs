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

        [Required(ErrorMessage = "Titlul este obligatoriu!")]
        [MinLength(2, ErrorMessage = "Titlul trebuie să conțină minim două caractere!")]
        [MaxLength(100, ErrorMessage = "Ttitlul poate să conțină maxim 100 de caractere!")]
        public string titlu { get; set; }

        [Required(ErrorMessage = "Descrierea este obligatorie!")]
        [MinLength(10, ErrorMessage = "Descrierea trebuie să conțină minim 10 caractere!")]
        [MaxLength(4096, ErrorMessage = "Descrierea trebuie să conțină maxim 4096 de caractere!")]
        public string descriere { get; set; }

        public virtual ICollection<CategorieProdus> CategoriiProduse { get; set; }
    }
}