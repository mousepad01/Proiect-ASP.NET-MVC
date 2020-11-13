using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Proiect_ASP.Models
{
    [Table("ProduseRatinguri")]
    public class ProdusRating
    {
        [Key]
        public int prodRating { get; set; }

        [Required]
        public int idProdus { get; set; }

        [Required]
        public int idUtilizator { get; set; }

        public DateTime dataReview { get; set; }

        [Required(ErrorMessage = "Nu puteți încărca un rating inexistent")]
        [Range(1, 5, ErrorMessage = "Rating-ul trebuie să fie între o stea și 5 stele")]
        public int rating { get; set; }

        [MaxLength(1024, ErrorMessage = "Detalierea rating-ului trebuie să aibă maxim 2014 caractere!")]
        public string descriere { get; set; }
       
        public virtual Produs produs { get; set; }
    }
}