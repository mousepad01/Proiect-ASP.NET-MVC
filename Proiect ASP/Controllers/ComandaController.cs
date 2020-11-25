using Proiect_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect_ASP.Controllers
{
    public class ComandaController : Controller
    {
        /*
         * Eu am facut operatiile CRUD normale pe entitatea asta, dar nu functioneaza deocamdata
         * Nici nu am terminat-o deoarece sunt inca nesigur de unele idei de implementare
         * Am explicat mai jos ce vreau sa spun, ca ea ar trebui sa aiba operatiile CRUD facute intr-un mod special
         */
        
        private Models.AppContext db = new Models.AppContext();

        // GET: Luam toate comenzile - teoretic doar admin-ul ar trebui sa vada asta
        // Ar trebui si un GET doar pentru un singur user, dar e redundant atunci cand nu avem userii implementati
        public ActionResult Index()
        {
            var comenzi = from a in db.Comenzi
                         select a;

            ViewBag.comenzi = comenzi;

            if (TempData["mesaj"] != null)
                ViewBag.mesaj = TempData["mesaj"];

            return View();
        }

        // La afisare ar trebui sa-si vada un utilizator o singura comanda
        public ActionResult Afisare(int id)
        {
            Comanda adresaCautata = db.Comenzi.Find(id);
            return View(adresaCautata);
        }

        // Editarea nu ar trebui sa existe pe comanda daca ea a fost finalizata (trimisa)
        // Admin-ul ar trebui sa poata sa seteze cand ea a fost trimisa (un buton prin care dataFinalizare = DateTime.Now();) si astfel ea nu mai poate fi editata de nimeni
        // O idee ar fi si ca adminul sa editeze comanda ca o "verificare" si astfel editare = trimitere, si setam atunci dataFinalizare
        public ActionResult Editare(int id)
        {
            Comanda comandaDeEditat = db.Comenzi.Find(id);
            comandaDeEditat.Adrese = GetAllAdresses();

            return View("FormEditare", comandaDeEditat);
        }
        
        [HttpPut]
        public ActionResult Editare(int id, Comanda comandaActualizata)
        {
            Comanda comandaDeEditat = db.Comenzi.Find(id);
            comandaActualizata.dataFinalizare = DateTime.Now;

            try
            {
                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(comandaDeEditat))
                    {
                        comandaDeEditat = comandaActualizata;
                        db.SaveChanges();

                        return RedirectToAction("Afisare", new { id = id });
                    }
                    else
                    {
                        return View("FormEditare", comandaActualizata);
                    }
                }
                else
                {
                    return View("FormEditare", comandaActualizata);
                }
            }
            catch (Exception e)
            {
                return View();
            }
        }

        // Adaugarea ar trebui sa fie atunci cand ai plasat comanda. Astfel s-ar sterge produsele din cos (nu stiu deocamdata cum le retinem, trebuie discutat)
        public ActionResult Adaugare()
        {
            Comanda comanda = new Comanda();
            comanda.Adrese = GetAllAdresses();

            return View("FormAdaugare", comanda);
        }

        [HttpPost]
        public ActionResult Adaugare(Comanda comandaDeAdaugat)
        {
            comandaDeAdaugat.dataPlasare = DateTime.Now;
            try
            {
                if (ModelState.IsValid)
                {
                    Comanda comandaAdaugata = db.Comenzi.Add(comandaDeAdaugat);
                    db.SaveChanges();

                    return RedirectToAction("Afisare", new { id = comandaAdaugata.idComanda });
                }
                else
                {
                    return View("FormAdaugare", comandaDeAdaugat);
                }
            }
            catch (Exception e)
            {
                return View();
            }
        }

        // Ar trebui sa fie posibil de facut doar de catre un admin
        [HttpDelete]
        public ActionResult Stergere(int id)
        {
            try
            {
                Comanda comandaDeSters = db.Comenzi.Find(id);
                db.Comenzi.Remove(comandaDeSters);

                db.SaveChanges();

                TempData["mesaj"] = "Comanda a fost stearsa cu succes!";

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View();
            }
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllAdresses()
        {
            var selectList = new List<SelectListItem>();
            var adrese = from a in db.Adrese
                             select a;

            foreach (var adresa in adrese)
            {
                selectList.Add(new SelectListItem
                {
                    Value = adresa.idAdresa.ToString(),
                    Text = adresa.strada.ToString() // Ar trebui adaugat un titlu al adresei pentru a putea fi gasita aici
                });
            }

            return selectList;
        }
    }
}