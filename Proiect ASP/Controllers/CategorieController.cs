using Proiect_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect_ASP.Controllers
{   
    public class CategorieController : Controller
    {
        private Models.AppContext db = new Models.AppContext();

        // GET: Lista categorii
        public ActionResult Index()
        {
            var categorii = from c in db.Categorii
                            select c;

            ViewBag.categorii = categorii;

            if (TempData["mesaj"] != null)
                ViewBag.mesaj = TempData["mesaj"];

            return View();
        }

        // GET: Afisarea unei categorii (si produsele asociate)
        public ActionResult Afisare(int id)
        {
            Categorie categorieCautata = db.Categorii.Find(id);

            //produsele asociate categoriei cautate (prin intermediul tabelei CategoriiProduse)
            var produse = from p in db.Produse
                          from cp in db.CategoriiProduse
                          where cp.idCategorie == categorieCautata.idCategorie
                          where p.idProdus == cp.idProdus
                          select p;

            ViewBag.produse = produse;

            return View(categorieCautata);
        }

        //GET: Afisarea form ului pentru editarea atributelor unei categorii
        public ActionResult Editare(int id)
        {
            Categorie categorieDeEditat = db.Categorii.Find(id);

            return View("FormEditare", categorieDeEditat);
        }

        [HttpPut]
        public ActionResult Editare(int id, Categorie categorieActualizata)
        {
            Categorie categorieDeEditat = db.Categorii.Find(id);

            try
            {
                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(categorieDeEditat))
                    {
                        categorieDeEditat = categorieActualizata;
                        db.SaveChanges();

                        return RedirectToAction("Afisare", new { id = id });
                    }
                    else
                    {
                        ViewBag.categorie = categorieDeEditat;
                        return View("FormEditare", categorieActualizata);
                    }
                }
                else
                {
                    categorieActualizata.idCategorie = categorieDeEditat.idCategorie;
                    return View("FormEditare", categorieActualizata);
                }
                
            }
            catch (Exception e)
            {
                ViewBag.exception = e;

                return View("ExceptieEditare");
            }
        }
           
        //GET: afisarea form ului de adaugare a unei noi categorii
        public ActionResult Adaugare()
        {
            Categorie categ = new Categorie();
            return View("FormAdaugare");
        }

        [HttpPost]
        public ActionResult Adaugare(Categorie categorieDeAdaugat)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Categorie categorieAdaugata = db.Categorii.Add(categorieDeAdaugat);  // returneaza obiectul adaugat
                    db.SaveChanges();

                    return RedirectToAction("Afisare", new { id = categorieAdaugata.idCategorie });
                }
                else
                {
                    return View("FormAdaugare", categorieDeAdaugat);
                }
            }
            catch (Exception e)
            {
                ViewBag.exceptie = e;

                return View("ExceptieAdaugare");
            }
        }
        [HttpDelete]
        public ActionResult Stergere(int id)
        {
            try
            {
                Categorie categorieDeSters = db.Categorii.Find(id);
                db.Categorii.Remove(categorieDeSters);

                db.SaveChanges();

                TempData["mesaj"] = "Categoria a fost ștearsă cu succes!";

                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                ViewBag.exceptie = e;

                return View("ExceptieStergere");
            }
        }
    }
}