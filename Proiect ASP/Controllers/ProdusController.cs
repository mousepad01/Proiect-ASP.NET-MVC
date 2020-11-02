using Proiect_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect_ASP.Controllers
{
    public class ProdusController : Controller
    {
        private Models.AppContext db = new Models.AppContext();

        // GET: Lista produse
        public ActionResult Index()
        {
            var produse = from p in db.Produse
                          select p;

            ViewBag.produse = produse;

            return View();
        }

        // GET: Afisarea unui produs (si a categoriilor asociate)
        public ActionResult Afisare(int id)
        {
            Produs produs = db.Produse.Find(id);

            //caategoriile din care produsul face parte
            var categorii = from cp in db.CategoriiProduse
                            from c in db.Categorii
                            where cp.idProdus == produs.idProdus
                            where cp.idCategorie == c.idCategorie
                            select c;

            ViewBag.produs = produs;
            ViewBag.categorii = categorii;

            return View();
        }

        // arunca exceptie daca strCategorii nu are formatul potrivit sau categoria nu exista
        [NonAction]
        public Categorie[] getCategorii(string strCategorii, ref int n_categ)
        {
            // procesez string ul in numele categoriilor, caut numele categoriilor in parte in baza de date pentru
            // a verifica daca exista, si dupa adaugarea noului obiect, adaug inregistrari noi 
            // in tabela CategoriiProduse

            string[] numeCategorii = new string[100];

            n_categ = 0;
            int i = 0;

            while (i < strCategorii.Length)
            {
                while (i < strCategorii.Length && strCategorii[i] != ',')
                {
                    if (strCategorii[i] != ' ')
                        numeCategorii[n_categ] += strCategorii[i];

                    i += 1;
                }

                i += 1;
                n_categ += 1;
            }

            Categorie[] listaCategorii = new Categorie[100];

            for (i = 0; i < n_categ; i++)
            {
                // daca faceam direct query cu c.titlu == numeCategorii[i] AR FI ARUNCAT EROARE
                // (nu trebuie sa am referinte in array uri in query uri de tip LINQ)
                // de asta am nevoie de variabila auxiliata numeCategorie, la prima vedere inutila

                var numeCategorie = numeCategorii[i];

                var categorieAsociata = from c in db.Categorii
                                        where c.titlu == numeCategorie
                                        select c;

                if (categorieAsociata != null)
                    listaCategorii[i] = categorieAsociata.SingleOrDefault();
                else
                    throw new ArgumentException("Categories provided in string do not exist", strCategorii);
            }

            return listaCategorii;
        }

        // Adauga in tabela CategoriiProduse inregistrari cu id-ul dat si categoriile preluate
        // (se are in vedere ca metoda care o apeleaza sterge vechile categorii de dinainte)
        //
        // Am ales sa implementez asa deoarece ar fi existat situatii cand trebuia sa sterg vechile categorii
        // Si in acelasi timp sa adaug altele noi, deci in cel mai rau caz oricum aveam de apelat doua metode
        // Pentru a simplifica implementarea, le apelez de fiecare data, 
        // Renuntand la ideea de a imparti situatia in mai multe cazuri
        [NonAction]
        public ActionResult AdaugaCategoriiAsociate(int id, int n_categ, Categorie[] categoriiDeAdaugat)
        {   
            try
            {
                for (int i = 0; i < n_categ; i++)
                {
                    var categIdAux = categoriiDeAdaugat[i].idCategorie;

                    var categProd = from cp in db.CategoriiProduse
                                    where cp.idCategorie == categIdAux
                                    where cp.idProdus == id
                                    select cp;

                    var auxCategProd = categProd.SingleOrDefault();

                    db.CategoriiProduse.Add(new CategorieProdus { idProdus = id, idCategorie = categoriiDeAdaugat[i].idCategorie });
                    db.SaveChanges();
                }

                return RedirectToAction("Afisare", new { id = id });
            }
            catch (Exception e)
            {
                ViewBag.exceptie = e;
                return View("ExceptieEditare");
            }
        }

        // Prima metoda (a doua fiind AdaugaCategoriiAsociate) 
        // folosita in vederea modificarii categoriilor din care un produs face parte
        // Aceasta metoda VERIFICA daca noile categorii sunt VALIDE, dupa care STERGE vechile categorii
        [HttpDelete]
        public ActionResult ModificaCategoriiAsociate(int id, string strCategorii)
        {
            try
            {
                int n_categ = 0;
                Categorie[] categoriiNoi = getCategorii(strCategorii, ref n_categ);

                var asocieri = from cp in db.CategoriiProduse
                               where cp.idProdus == id
                               select cp;

                foreach (var asoc in asocieri)
                    db.CategoriiProduse.Remove(asoc);

                db.SaveChanges();

                return AdaugaCategoriiAsociate(id, n_categ, categoriiNoi);
            }
            catch (Exception e)
            {
                ViewBag.exceptie = e;

                return View("ExitareNereusita");
            }
        }

        //GET: Afisarea form ului pentru editarea unui produs (FARA CATEGORIILE ASOCIATE)
        public ActionResult Editare(int id)
        {
            Produs produsDeEditat = db.Produse.Find(id);

            var categorii = from cp in db.CategoriiProduse
                            from c in db.Categorii
                            where cp.idProdus == produsDeEditat.idProdus
                            where cp.idCategorie == c.idCategorie
                            select c;

            string strCategorii = "";

            foreach (var categ in categorii)
                strCategorii += categ.titlu + ", ";

            ViewBag.strCategorii = strCategorii;

            ViewBag.produs = produsDeEditat;

            return View("FormEditare");
        }

        [HttpPut]
        public ActionResult Editare(int id, string strCategorii, Produs produsActualizat)
        {
            Produs produsDeEditat = db.Produse.Find(id);

            try
            { 
                if (TryUpdateModel(produsDeEditat))
                {
                    produsDeEditat = produsActualizat;
                    db.SaveChanges();

                    return RedirectToAction("Afisare", new { id = id });
                }
                else
                {
                    ViewBag.produs = produsDeEditat;
                    return View("EditareNereusita");
                }
            }
            catch(Exception e)
            {
                ViewBag.exceptie = e;

                return View("ExceptieEditare");
            }
        }

        //GET: afisarea form ului pentru adaugarea unui produs nou
        public ActionResult Adaugare()
        {   
            return View("FormAdaugare");
        }

        [HttpPost]
        public ActionResult Adaugare(Produs produsDeAdaugat, string strCategorii)
        {
            //cu ajutorul input ului care are proprietatea name="strCategorii", se va transmite in acest parametru acea valoare

            try
            {
                int n_categ = 0;

                Categorie[] categorii = getCategorii(strCategorii, ref n_categ);

                produsDeAdaugat.dataAdaugare = DateTime.Now;

                db.Produse.Add(produsDeAdaugat);

                for(int i = 0; i < n_categ; i++)
                    db.CategoriiProduse.Add(new CategorieProdus { idProdus = produsDeAdaugat.idProdus, idCategorie = categorii[i].idCategorie });

                db.SaveChanges();

                return RedirectToAction("Afisare", new { id = produsDeAdaugat.idProdus });
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
                Produs produsDeSters = db.Produse.Find(id);
                db.Produse.Remove(produsDeSters);

                db.SaveChanges();

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