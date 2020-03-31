using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebshopGraphicsCard.Models;
using WebshopGraphicsCard.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace WebshopGraphicsCard.Controllers
{
    public class HomeController : Controller
    {
        
        PersistenceCode PC = new PersistenceCode();

        //De catalogus ophalen
        [Authorize]
        [HttpGet]
        public IActionResult Index(ArtikelRepository ArtRepo)
        {
            Klant klant = new Klant();
            HttpContext.Session.SetString("KlantNr", Convert.ToString(klant.KlantNr));
            
            ArtRepo.Artikels = PC.loadArtikels();
            return View(ArtRepo);
        }

        [Authorize]
        public IActionResult Index(VMWinkelmand vMWinkelmand)
        {
            return RedirectToAction("Winkelmandje");
        }

        [Authorize]
        [HttpGet]
       public IActionResult Toevoegen(int ArtNr)
       {
            VMToevoegen vMToevoegen = new VMToevoegen();
            vMToevoegen.artikel= PC.loadArtikel(ArtNr);
            HttpContext.Session.SetString("ArtNr", ArtNr.ToString());
            return View(vMToevoegen);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Toevoegen(VMToevoegen vMToevoegen)
        {
            Artikel art = new Artikel();
            vMToevoegen.artikel = PC.loadArtikel(Convert.ToInt32(HttpContext.Session.GetString("ArtNr")));
            Winkelmand winkelmand = new Winkelmand();
            if (ModelState.IsValid)
            {                  
                if ((vMToevoegen.Aantal > 0) && (vMToevoegen.Aantal <= vMToevoegen.artikel.Voorraad ))
                {
                    winkelmand.KlantNr = Convert.ToInt32(HttpContext.Session.GetString("KlantNr"));
                    winkelmand.Aantal = vMToevoegen.Aantal;
                    winkelmand.ArtNr = Convert.ToInt32(HttpContext.Session.GetString("ArtNr"));
                    PC.PasMandjeAan(winkelmand);
                    return RedirectToAction("Winkelmandje");
                }
                else
                {
                    ViewBag.fout = "Het aantal dat u ingeeft is niet correct.";
                    return View(vMToevoegen);
                }
            }
            else
            {
                
                return View(vMToevoegen);
            }           
        }

        [Authorize]
        [HttpGet]
        public IActionResult Winkelmandje(VMWinkelmand vMWinkelmand)
        {
            int klantnr = Convert.ToInt32(HttpContext.Session.GetString("KlantNr"));
            vMWinkelmand.Klant = PC.loadKlant(Convert.ToInt32(klantnr));
            if (PC.ControleerWinkelmand(klantnr) == true)
            {
                ViewBag.Leeg = true;               
                WinkelmandRepository winkelRepo = new WinkelmandRepository();
                winkelRepo.WinkelmandItems = PC.loadWinkelmand(klantnr);
                vMWinkelmand.WinkelmandRepo = winkelRepo;
                vMWinkelmand.BerekendGegeven = PC.BerekenTotaal();
                HttpContext.Session.SetString("TotaalInclu", Convert.ToString(vMWinkelmand.BerekendGegeven.TotaalInclu));

            }
            else
            {
                ViewBag.Leeg = false;
            }

            return View(vMWinkelmand);
            
        }

        [Authorize]
        [HttpGet]
        public IActionResult DeleteItem(int ArtNr, int Aantal)
        {
            Winkelmand winkelmand = new Winkelmand();
            winkelmand.ArtNr = ArtNr;
            winkelmand.KlantNr = Convert.ToInt32(HttpContext.Session.GetString("KlantNr"));
            winkelmand.Aantal = Aantal;
            PC.DeleteWinkelmandItem(winkelmand);
            return RedirectToAction("Winkelmandje");
        }

        [Authorize]
        [HttpPost]
        public IActionResult Winkelmandje(VMbestelling vMbestelling)
        {
            int klantnr = Convert.ToInt32(HttpContext.Session.GetString("KlantNr"));
            vMbestelling.klant = PC.loadKlant(klantnr);
            vMbestelling.Bestelling = PC.MaakBestelling(klantnr);
            double TotaalInclu = Convert.ToDouble(HttpContext.Session.GetString("TotaalInclu"));
            ViewBag.TotaalInclu = TotaalInclu;
            vMbestelling.Verzend(TotaalInclu);
            return RedirectToAction("Bevestiging");
        }

        [Authorize]
        public IActionResult Bevestiging(VMbestelling vMbestelling)
        {
            int klantnr = Convert.ToInt32(HttpContext.Session.GetString("KlantNr"));
            vMbestelling.klant = PC.loadKlant(klantnr);
            vMbestelling.Bestelling = PC.MaakBestelling(klantnr);
            double TotaalInclu = Convert.ToDouble(HttpContext.Session.GetString("TotaalInclu"));
            ViewBag.TotaalInclu = TotaalInclu;
            

            return View(vMbestelling);
        }
    }
}
