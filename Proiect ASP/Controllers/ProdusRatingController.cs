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

        // GET : apelat de un script in jQuery, returneaza view ul partial pentru adaugarea unui comentariu nou
        // in aceasta metoda, parametrul ID este AL PRODUSULUI ASOCIAT !!!!!
        public ActionResult AdaugaRating(int id)
        {
            ProdusRating ratingDeAdaugat = new ProdusRating();
            ratingDeAdaugat.idProdus = id;

            return PartialView("InputRating", ratingDeAdaugat);
        }

        // in aceasta metoda, parametrul ID este AL PRODUSULUI ASOCIAT !!!!!
        [HttpPost]
        public ActionResult AdaugaRating(int id, ProdusRating ratingDeAdaugat)
        {
            ratingDeAdaugat.idProdus = id;
            ratingDeAdaugat.dataReview = DateTime.Now;

            // initializez idUtilizator cu o valoare random pentru a nu avea probleme la momentul actual
            ratingDeAdaugat.idUtilizator = 123;

            try
            {
                if (ModelState.IsValid)
                {
                    db.ProduseRatinguri.Add(ratingDeAdaugat);
                    db.SaveChanges();

                    return Redirect("/Produs/Afisare/" + ratingDeAdaugat.idProdus);
                }
                else
                {
                    TempData["eroareRatingAdaugat"] = true;
                    TempData["ratingEronatAdaugat"] = ratingDeAdaugat;
                    TempData["eroare"] = "Rating-ul trebuie să fe între 1 și 5 stele, si descrierea nu mai lungă de 1024 de caractere!";
                    
                    return Redirect("/Produs/Afisare/" + ratingDeAdaugat.idProdus);
                }
            }
            catch(Exception)
            {
                TempData["mesaj"] = "Eroare la adaugarea unui comentariu nou";
                return Redirect("/Produs/Afisare/" + ratingDeAdaugat.idProdus);
            }
        }

        [HttpDelete]
        public ActionResult StergeRating(int id)
        {
            ProdusRating ratingDeSters = db.ProduseRatinguri.Find(id);

            int produsAsociat = ratingDeSters.idProdus;

            try
            {
                db.ProduseRatinguri.Remove(ratingDeSters);

                db.SaveChanges();

                TempData["mesaj"] = "Comentariul a fost șters cu succes!";

                return Redirect("/Produs/Afisare/" + produsAsociat);
            }
            catch (Exception)
            {
                TempData["mesaj"] = "Eroare la ștergerea unui comentariu";
                return Redirect("/Produs/Afisare/" + produsAsociat);
            }
        }

        // GET : apelat de un script in jQuery, afiseaza form ul de editare al unui review
        public ActionResult EditeazaRating(int id)
        {
            ProdusRating ratingDeEditat = db.ProduseRatinguri.Find(id);

            return PartialView("EditareRating", ratingDeEditat);
        }

        [HttpPut]
        public ActionResult EditeazaRating(int id, ProdusRating ratingActualizat)
        {
            ProdusRating ratingDeEditat = db.ProduseRatinguri.Find(id);

            try
            {
                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(ratingDeEditat))
                    {
                        ratingDeEditat.rating = ratingActualizat.rating;
                        ratingDeEditat.descriere = ratingActualizat.descriere;
                        ratingDeEditat.dataReview = DateTime.Now;

                        db.SaveChanges();

                        return Redirect("/Produs/Afisare/" + ratingDeEditat.idProdus);
                    }
                    else
                    {
                        TempData["mesaj"] = "Nu s-a putut edita review-ul";
                        return Redirect("/Produs/Afisare/" + ratingDeEditat.idProdus);
                    }
                }
                else
                {
                    TempData["eroareRatingEditat"] = true;
                    ratingActualizat.prodRating = id;
                    TempData["ratingEronatEditat"] = ratingActualizat;
                    TempData["eroare"] = "Rating-ul trebuie să fe între 1 și 5 stele, si descrierea nu mai lungă de 1024 de caractere!";

                    return Redirect("/Produs/Afisare/" + ratingDeEditat.idProdus);
                }
            }
            catch (Exception)
            {
                TempData["mesaj"] = "Eroare la editarea review-ului!";
                return Redirect("/Produs/Afisare/" + ratingDeEditat.idProdus);
            }
        }
    }
}