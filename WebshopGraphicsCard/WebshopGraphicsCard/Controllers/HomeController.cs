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

namespace WebshopGraphicsCard.Controllers
{
    public class HomeController : Controller
    {
        
        PersistenceCode PC = new PersistenceCode();

        [HttpGet]
        public IActionResult Index()
        {
            Klant klant = new Klant();
            HttpContext.Session.SetString("KlantNr", "1");
            ArtikelRepository ArtRepo = new ArtikelRepository();
            ArtRepo.Artikels = PC.loadArtikels();
            return View(ArtRepo);
        }

        
        public IActionResult Index(VMWinkelmand vMWinkelmand)
        {
            return RedirectToAction("Winkelmandje");
        }

       [HttpGet]
       public IActionResult Toevoegen(int ArtNr)
       {
            VMToevoegen vMToevoegen = new VMToevoegen();
            vMToevoegen.artikel= PC.loadArtikel(ArtNr);
            HttpContext.Session.SetString("ArtNr", ArtNr.ToString());
            return View(vMToevoegen);
        }

        
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
                
            }
            else
            {
                ViewBag.Leeg = false;
            }

            return View(vMWinkelmand);
            
        }

       
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

        [HttpPost]
        public IActionResult Winkelmandje(VMbestelling vMbestelling)
        {
            int klantnr = Convert.ToInt32(HttpContext.Session.GetString("KlantNr"));
            vMbestelling.klant = PC.loadKlant(klantnr);
            vMbestelling.Bestelling = PC.MaakBestelling(klantnr);
            BerekendGegeven berekendGegeven = new BerekendGegeven();
            double totaal = berekendGegeven.TotaalInclu;
            ViewBag.Totaal = totaal;
            vMbestelling.Verzend();
            return RedirectToAction("Bevestiging");
        }

        public IActionResult Bevestiging(VMbestelling vMbestelling)
        {

            return View(vMbestelling);
        }
    }
}
