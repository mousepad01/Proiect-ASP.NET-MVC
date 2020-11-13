using Proiect_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect_ASP.Controllers
{
    public class ProdusRatingController : Controller
    {
        private Models.AppContext db = new Models.AppContext();

        [HttpPost]
        public ActionResult AdaugaRating(int id, ProdusRating ratingDeAdaugat)
        {
            ratingDeAdaugat.idProdus = id;
            ratingDeAdaugat.dataReview = DateTime.Now;

            // initializez idUtilizator cu o valoare random pentru a nu avea probleme la momentul actual
            ratingDeAdaugat.idUtilizator = 123;

            try
            {
                db.ProduseRatinguri.Add(ratingDeAdaugat);
                db.SaveChanges();

                return Redirect("/Produs/Afisare/" + ratingDeAdaugat.idProdus);
            }
            catch(Exception)
            {
                //trebuie sa implementam ceva aici care sa aiba sens, pun asa numa ca sa mearga pe moment
                return Redirect("/Produs/Afisare/" + ratingDeAdaugat.idProdus);
            }
        }
    }
}