using Proiect_ASP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

            // pentru a afisa intervalele default de filtre
            int pretMax = produse.Max(produs => produs.pret);
            int pretMin = produse.Min(produs => produs.pret);

            DateTime dataMax = produse.Max(produs => produs.dataAdaugare);
            DateTime dataMin = produse.Min(produs => produs.dataAdaugare);

            ViewBag.produse = produse;

            ViewBag.pretMax = pretMax;
            ViewBag.pretMin = pretMin;


            ViewBag.dataMax = dataMax;
            ViewBag.dataMin = dataMin;

            if (TempData["mesaj"] != null)
                ViewBag.mesaj = TempData["mesaj"];

            return View();
        }

        // POST PENTRU CĂ AȘA VREA FORMCOLLECTION, DAR PUTEA SĂ FIE GET: Lista produse sortate
        [HttpPost]
        public ActionResult IndexSorted(FormCollection result)
        {
            // FormCollection permite form-ului să fie transmis mai fancy și modificat ușor

            // Intervalele default din filtre 
            var produse = from p in db.Produse
                          select p;
            int pretMax = produse.Max(produs => produs.pret);
            int pretMin = produse.Min(produs => produs.pret);
            DateTime dataMax = produse.Max(produs => produs.dataAdaugare);
            DateTime dataMin = produse.Min(produs => produs.dataAdaugare);
            ViewBag.pretMax = pretMax;
            ViewBag.pretMin = pretMin;
            ViewBag.dataMax = dataMax;
            ViewBag.dataMin = dataMin;
            
            int pretMinAux = Convert.ToInt16(result["pretMin"]);
            int pretMaxAux = Convert.ToInt16(result["pretMax"]);
            DateTime dataMinAux = Convert.ToDateTime(result["dataMin"]);
            DateTime dataMaxAux = Convert.ToDateTime(result["dataMax"]);

            // O sa ma gandesc eu la niste chestii sa nu arunce o exceptie random daca nu avem vreun pret setat, dar selectam checkbox-ul de pret (idem pentru data)
            if (result["consideraData"] != null && result["consideraPret"] != null)
            {
                produse = from p in db.Produse
                          where p.pret >= pretMinAux && p.pret <= pretMaxAux
                          where p.dataAdaugare >= dataMinAux && p.dataAdaugare <= dataMaxAux
                          select p;
            }
            else if (result["consideraData"] != null)
            {
                produse = from p in db.Produse
                          where p.dataAdaugare >= dataMinAux && p.dataAdaugare <= dataMaxAux
                          select p;
            }
            else if (result["consideraPret"] != null)
            {
                produse = from p in db.Produse
                          where p.pret >= pretMinAux && p.pret <= pretMaxAux
                          select p;
            }

            if (result["sortCrit"] == "tc")
                produse = produse.OrderBy(produs => produs.titlu);
            else if (result["sortCrit"] == "tdc")  
                produse = produse.OrderByDescending(produs => produs.titlu);
            else if (result["sortCrit"] == "pc")
                produse = produse.OrderBy(produs => produs.pret);
            else if (result["sortCrit"] == "pdc")
                produse = produse.OrderByDescending(produs => produs.pret);
            else if (result["sortCrit"] == "dc")
                produse = produse.OrderBy(produs => produs.dataAdaugare);
            else if (result["sortCrit"] == "ddc")
                produse = produse.OrderByDescending(produs => produs.dataAdaugare);

            ViewBag.produse = produse;

            return View("Index");
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


        // Dle. Proteina, cand va angajati sa cereti salariul direct proportional cu nr. de linii scrise =))))
        /*
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
        }*/

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
        // STERGE vechile categorii
        [HttpDelete]
        public ActionResult ModificaCategoriiAsociate(int id, int[] categoriiActualizateId)
        {
            try
            {
                Categorie[] categoriiActualizate = new Categorie[100];

                for(int i = 0; i < categoriiActualizateId.Length; i++)
                {
                    //System.Diagnostics.Debug.WriteLine(categoriiActualizateId[i]);
                    int categId = categoriiActualizateId[i];

                    var categ = from c in db.Categorii
                                where c.idCategorie == categId
                                select c;

                    categoriiActualizate[i] = categ.SingleOrDefault();
                    //System.Diagnostics.Debug.WriteLine(categoriiActualizate[i].titlu);
                }

                var asocieri = from cp in db.CategoriiProduse
                               where cp.idProdus == id
                               select cp;

                foreach (var asoc in asocieri)
                    db.CategoriiProduse.Remove(asoc);

                db.SaveChanges();

                return AdaugaCategoriiAsociate(id, categoriiActualizateId.Length, categoriiActualizate);
            }
            catch (Exception e)
            {
                ViewBag.exceptie = e;

                return View("EditareNereusita");
            }
        }

        //GET: Afisarea form ului pentru editarea unui produs 
        public ActionResult Editare(int id)
        {
            Produs produsDeEditat = db.Produse.Find(id);

            var categoriiAsociate = from cp in db.CategoriiProduse
                                    from c in db.Categorii
                                    where cp.idProdus == produsDeEditat.idProdus
                                    where cp.idCategorie == c.idCategorie
                                    select c;

            // delimitez categoriile neasociate de cele asociate
            var categoriiNeasociate = (from c in db.Categorii
                                       select c).Except(categoriiAsociate);

            ViewBag.categoriiNeasociate = categoriiNeasociate;

            ViewBag.categoriiAsociate = categoriiAsociate;

            ViewBag.produs = produsDeEditat;

            return View("FormEditare");
        }

        // Editarea informatiilor unui produs FARA CATEGORIILE ASOCIATE
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
            var categorii = from c in db.Categorii
                            select c;

            ViewBag.categorii = categorii;

            return View("FormAdaugare");
        }

        [HttpPost]
        public ActionResult Adaugare(Produs produsDeAdaugat, int[] categoriiDeAdaugatId)
        { 
            try
            {
                Categorie[] categoriiDeAdaugat = new Categorie[100];

                for (int i = 0; i < categoriiDeAdaugatId.Length; i++)
                {
                    int categId = categoriiDeAdaugatId[i];

                    var categ = from c in db.Categorii
                                where c.idCategorie == categId
                                select c;

                    categoriiDeAdaugat[i] = categ.SingleOrDefault();
                }

                produsDeAdaugat.dataAdaugare = DateTime.Now;

                db.Produse.Add(produsDeAdaugat);

                for(int i = 0; i < categoriiDeAdaugatId.Length; i++)
                    db.CategoriiProduse.Add(new CategorieProdus { idProdus = produsDeAdaugat.idProdus, idCategorie = categoriiDeAdaugat[i].idCategorie });

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

                TempData["mesaj"] = "Produsul a fost eliminat cu scces!";

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