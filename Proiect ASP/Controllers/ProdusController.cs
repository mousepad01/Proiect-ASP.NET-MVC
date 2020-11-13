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

        // GET: Lista produses
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

            ViewBag.pretMinDefault = pretMin;
            ViewBag.pretMaxDefault = pretMax;

            ViewBag.dataMax = dataMax;
            ViewBag.dataMin = dataMin;

            ViewBag.dataMaxDefault = dataMax;
            ViewBag.dataMinDefault = dataMin;

            ViewBag.selectedSort = "none";

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

            int pretMinAux = Convert.ToInt32(result["pretMin"]);
            int pretMaxAux = Convert.ToInt32(result["pretMax"]);
            DateTime dataMinAux = Convert.ToDateTime(result["dataMin"]);
            DateTime dataMaxAux = Convert.ToDateTime(result["dataMax"]);

            ViewBag.pretMinDefault = pretMinAux;
            ViewBag.pretMaxDefault = pretMaxAux;

            ViewBag.dataMaxDefault = dataMaxAux;
            ViewBag.dataMinDefault = dataMinAux;

            produse = from p in db.Produse
                        where p.pret >= pretMinAux && p.pret <= pretMaxAux
                        where p.dataAdaugare >= dataMinAux && p.dataAdaugare <= dataMaxAux
                        select p;

            string sortCrit = result["sortCrit"];

            if (sortCrit == "tc")
                produse = produse.OrderBy(produs => produs.titlu);
            else if (sortCrit == "tdc")
                produse = produse.OrderByDescending(produs => produs.titlu);
            else if (sortCrit == "pc")
                produse = produse.OrderBy(produs => produs.pret);
            else if (sortCrit == "pdc")
                produse = produse.OrderByDescending(produs => produs.pret);
            else if (sortCrit == "dc")
                produse = produse.OrderBy(produs => produs.dataAdaugare);
            else if (sortCrit == "ddc")
                produse = produse.OrderByDescending(produs => produs.dataAdaugare);

            ViewBag.selectedSort = sortCrit;

            ViewBag.produse = produse;

            return View("Index");
        }

        // Returneaza o lista cu obiecte de tipul (valoare, text) 
        // unde valoarea este ID ul categoriei asociate (sub forma de string)
        // iar textul este TITLUL categoriei asociate (sub forma de string)
        [NonAction]
        public IEnumerable <SelectListItem> categoriiAsociate(Produs produs)
        {
            var categoriiAsociateQuery = from cp in db.CategoriiProduse
                                         from c in db.Categorii
                                         where cp.idProdus == produs.idProdus
                                         where cp.idCategorie == c.idCategorie
                                         select c;

            var categoriiAsociate = new List <SelectListItem>();
            
            foreach(var categ in categoriiAsociateQuery)
            {
                categoriiAsociate.Add(new SelectListItem
                {
                    Value = categ.idCategorie.ToString(),
                    Text = categ.titlu.ToString()
                });
            }

            return categoriiAsociate;
        }

        // Returneaza o lista cu obiecte de tipul (valoare, text) 
        // unde valoarea este ID ul categoriei neasociate (sub forma de string)
        // iar textul este TITLUL categoriei neasociate (sub forma de string)
        [NonAction]
        public IEnumerable <SelectListItem> categoriiNeasociate(Produs produs)
        {
            var categoriiAsociateQuery = from cp in db.CategoriiProduse
                                         from c in db.Categorii
                                         where cp.idProdus == produs.idProdus
                                         where cp.idCategorie == c.idCategorie
                                         select c;

            var categoriiNeasociateQuery = (from c in db.Categorii
                                            select c).Except(categoriiAsociateQuery);

            var categoriiNeasociate = new List <SelectListItem>();

            foreach(var categ in categoriiNeasociateQuery)
            {
                categoriiNeasociate.Add(new SelectListItem
                {
                    Value = categ.idCategorie.ToString(),
                    Text = categ.titlu.ToString()
                });
            }

            return categoriiNeasociate;
        }

        // overload pe metoda categoriiNeasociate, care primeste in plus lista de categorii asociate
        [NonAction]
        public IEnumerable <SelectListItem> categoriiNeasociate(Produs produs, IEnumerable <SelectListItem> categoriiAsociate)
        {
            var categoriiNeasociate = new List <SelectListItem>();
            
            var categoriiQuery = from c in db.Categorii
                                 select c;

            foreach(var categ in categoriiQuery)
            {
                var categPending = new SelectListItem{ Value = categ.idCategorie.ToString(), Text = categ.titlu.ToString()};

                if (!categoriiAsociate.Any(c => c.Value == categPending.Value))
                    categoriiNeasociate.Add(categPending);
            }

            return categoriiNeasociate;
        }

        // metoda care returneaza un array de categorii asociat unui array de ID uri primite ca parametru
        [NonAction]
        public Categorie[] categoriiDinId(int[] categoriiId)
        {   
            Categorie[] categoriiCorespunzatoare = new Categorie[categoriiId.Length];

            for (int i = 0; i < categoriiId.Length; i++)
            {
                int categId = categoriiId[i];

                var categ = from c in db.Categorii
                            where c.idCategorie == categId
                            select c;

                categoriiCorespunzatoare[i] = categ.SingleOrDefault();
            }

            return categoriiCorespunzatoare;
        }

        // overload pe metoda categoriiAsociate, care primeste in plus lista de ID uri ale categoriilor asociate
        // si returneaza obiectul de tip IEnumerable corespunzator array ului de ID uri trimis ca parametru
        [NonAction]
        public IEnumerable <SelectListItem> categoriiAsociate(Produs produs, int[] categoriiAsociateId)
        {
            Categorie[] categoriiAsociateArray;

            var categoriiAsociateRez = new List<SelectListItem>();

            try
            {
                categoriiAsociateArray = categoriiDinId(categoriiAsociateId);

                for (int i = 0; i < categoriiAsociateArray.Length; i++)
                {
                    categoriiAsociateRez.Add(new SelectListItem
                    {
                        Value = categoriiAsociateArray[i].idCategorie.ToString(),
                        Text = categoriiAsociateArray[i].titlu.ToString()
                    });
                }

                return categoriiAsociateRez;
            }
            catch (NullReferenceException)
            {
                return categoriiAsociateRez;
            }
            
           
        }

        // GET: Afisarea unui produs (si a categoriilor asociate)
        public ActionResult Afisare(int id)
        {
            Produs produsDeAfisat = db.Produse.Find(id);

            produsDeAfisat.CategoriiAsociate = categoriiAsociate(produsDeAfisat);

            return View(produsDeAfisat);
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

        //GET: Afisarea form ului pentru editarea unui produs INCLUSIV CATEGORIILE ASOCIATE
        public ActionResult Editare(int id)
        {
            Produs produsDeEditat = db.Produse.Find(id);
            
            produsDeEditat.CategoriiNeasociate = categoriiNeasociate(produsDeEditat);
            produsDeEditat.CategoriiAsociate = categoriiAsociate(produsDeEditat);

            return View("FormEditare", produsDeEditat);
        }

        // Editarea informatiilor unui produs FARA CATEGORIILE ASOCIATE
        [HttpPut]
        public ActionResult Editare(int id, Produs produsActualizat)
        {
            Produs produsDeEditat = db.Produse.Find(id);

            try
            {
                if (ModelState.IsValid)
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
                else
                {
                    /*produsDeEditat.CategoriiNeasociate = categoriiNeasociate(produsDeEditat);
                    produsDeEditat.CategoriiAsociate = categoriiAsociate(produsDeEditat);*/
                    produsActualizat.CategoriiNeasociate = categoriiNeasociate(produsDeEditat, produsActualizat.CategoriiAsociate);
                    System.Diagnostics.Debug.WriteLine(produsActualizat.CategoriiNeasociate.Count());
                    System.Diagnostics.Debug.WriteLine(produsActualizat.CategoriiAsociate.Count());
                    return View("FormEditare", produsActualizat);
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
            Produs produs = new Produs();

            // gasirea categoriilor asociate nu este necesara
            // intrucat produsul trimis catre view este nou
            produs.CategoriiAsociate = categoriiAsociate(produs);
            produs.CategoriiNeasociate = categoriiNeasociate(produs);

            return View("FormAdaugare", produs);
        }

        [HttpPost]
        public ActionResult Adaugare(Produs produsDeAdaugat, int[] categoriiDeAdaugatId)
        { 
            try
            {
                produsDeAdaugat.CategoriiAsociate = categoriiAsociate(produsDeAdaugat, categoriiDeAdaugatId);

                if (ModelState.IsValid && produsDeAdaugat.CategoriiAsociate.Count() > 0)
                {
                    Categorie[] categoriiDeAdaugat = categoriiDinId(categoriiDeAdaugatId);

                    produsDeAdaugat.dataAdaugare = DateTime.Now;

                    db.Produse.Add(produsDeAdaugat);

                    for (int i = 0; i < categoriiDeAdaugatId.Length; i++)
                        db.CategoriiProduse.Add(new CategorieProdus { idProdus = produsDeAdaugat.idProdus, idCategorie = categoriiDeAdaugat[i].idCategorie });

                    db.SaveChanges();

                    return RedirectToAction("Afisare", new { id = produsDeAdaugat.idProdus });
                }
                else
                {
                    produsDeAdaugat.CategoriiNeasociate = categoriiNeasociate(produsDeAdaugat, produsDeAdaugat.CategoriiAsociate);

                    if(produsDeAdaugat.CategoriiAsociate.Count() == 0)
                        ViewBag.eroareCategoriiLipsa = "Produsul trebuie să facă parte din cel puțin o categorie!";

                    return View("FormAdaugare", produsDeAdaugat);
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