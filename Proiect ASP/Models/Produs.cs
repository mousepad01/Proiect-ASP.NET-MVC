using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect_ASP.Models
{
    [Table("Produse")]
    public class Produs
    {
        [Key]
        public int idProdus { get; set; }

        [Required(ErrorMessage = "Id-ul producătorului este obligatoriu!")]
        public int idOwner { get; set; }

        [Required(ErrorMessage = "Titlul produsului este obligatoriu!")]
        [MinLength(2, ErrorMessage = "Titlul trebuie să conțină minim două caractere!")]
        [MaxLength(100, ErrorMessage = "Titlul trebuie să conțină cel mult 100 de caractere!")]
        public string titlu { get; set; }

        [Required(ErrorMessage = "Descrierea produsului este obligatorie!")]
        [MinLength(10, ErrorMessage = "Descrierea trebuie să conțină cel puțin 10 caractere!")]
        [MaxLength(4096, ErrorMessage = "Descrierea trebuie să conțină cel mult 4096 de caractere!")]
        public string descriere { get; set; }
        public string imagePath { get; set; }

        [Required(ErrorMessage = "Prețul produsului este obligatoriu!")]
        [Range(0, 1000000000, ErrorMessage = "Prețul trebuie să fie cuprins între 0 și 1 miliard!")]
        public int pret { get; set; }

        public DateTime dataAdaugare { get; set; }

        [Required(ErrorMessage = "Cantitatea produsului este obligatorie!")]
        [Range(1, 1000000, ErrorMessage = "Cantitatea trebuie să fie cuprinsă între 1 și 1 milion!")]
        public int cantitate { get; set; }

        public virtual ICollection<CategorieProdus> CategoriiProduse { get; set; }
        public virtual ICollection<ProdusRating> ProduseRatinguri { get; set; }

        //atribute pentru categoriile asociate si neasociate pentru a le folosi in form-uri
        public IEnumerable <SelectListItem> CategoriiAsociate { get; set; }
        public IEnumerable <SelectListItem> CategoriiNeasociate { get; set; }

        public IEnumerable <SelectListItem> Ratinguri { get; set; }
    }
}