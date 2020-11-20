using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect_ASP.Models
{

    [Table("Comenzi")]
    public class Comanda
    {
        [Key]
        public int idComanda { get; set; }

        [Required]
        public DateTime dataPlasare { get; set; }

        [Required]
        public int idAdresa { get; set; }

        [Required]
        public int idUtilizator { get; set; }

        public DateTime dataFinalizare { get; set; }

        public virtual Adresa Adresa { get; set; }

        public IEnumerable<SelectListItem> Adrese { get; set; }
    }
}