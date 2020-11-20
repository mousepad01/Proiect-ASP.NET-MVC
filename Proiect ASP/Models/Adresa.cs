using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Proiect_ASP.Models
{

    [Table("Adrese")]
    public class Adresa
    {
        [Key]
        public int idAdresa { get; set; }

        [Required(ErrorMessage = "Tara este obligatorie!")]
        [MinLength(3, ErrorMessage = "Tara trebuie să conțină minim trei caractere!")]
        [MaxLength(25, ErrorMessage = "Tara poate să conțină maxim 25 de caractere!")]
        public string tara { get; set; }

        [Required(ErrorMessage = "Orasul este obligatorie!")]
        [MinLength(3, ErrorMessage = "Orasul trebuie să conțină minim trei caractere!")]
        [MaxLength(100, ErrorMessage = "Orasul poate să conțină maxim 100 de caractere!")]
        public string oras { get; set; }

        [Required(ErrorMessage = "Tara este obligatorie!")]
        [MinLength(3, ErrorMessage = "Tara trebuie să conțină minim trei caractere!")]
        [MaxLength(100, ErrorMessage = "Tara poate să conțină maxim 100 de caractere!")]
        public string strada { get; set; }

        [Required(ErrorMessage = "Nr. casei este obligatoriu!")]
        [Range(0, 1000, ErrorMessage = "Nr. casei trebuie să fie cuprins între 0 și 1000!")]
        public int nr { get; set; }

        [Required]
        public int idUtilizator { get; set; }

        public string mentiune { get; set; }

    }
}