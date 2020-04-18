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
        

        //De catalogus ophalen met alle artikelen en zijn prijs. 
        [Authorize]
        [HttpGet]
        public IActionResult Index(ArtikelRepository ArtRepo)
        {                   
            ArtRepo.Artikels = PC.loadArtikels();
            return View(ArtRepo);
        }

        // Om rechtsreeks naar het winkelmandje te gaan via een knop.
        [Authorize]     
        public IActionResult Index(VMWinkelmand vMWinkelmand)
        {
            return RedirectToAction("Winkelmandje");
        }

        //Een artikel laden om deze te bekijken en toe tevoegen aan het winkelmandje.
        [Authorize]
        [HttpGet]
        public IActionResult Toevoegen(int ArtNr)
        {
            VMToevoegen vMToevoegen = new VMToevoegen();
            vMToevoegen.artikel= PC.loadArtikel(ArtNr);
            HttpContext.Session.SetString("ArtNr", ArtNr.ToString());
            return View(vMToevoegen);
        }


        //Het aantal van het bepaalde artikel, toe tevoegen aan het winkelmandje.
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
                    winkelmand.KlantNr = Convert.ToInt32(HttpContext.Session.GetString("user"));
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


        // Het winkelmandje inladen met zijn artikkels.
        [Authorize]
        [HttpGet]
        public IActionResult Winkelmandje(VMWinkelmand vMWinkelmand)
        {
            int klantnr = Convert.ToInt32(HttpContext.Session.GetString("user"));
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

        // Een artikel uit het winkelmandje verwijderen.
        [Authorize]
        [HttpGet]
        public IActionResult DeleteItem(int ArtNr, int Aantal)
        {
            Winkelmand winkelmand = new Winkelmand();
            winkelmand.ArtNr = ArtNr;
            winkelmand.KlantNr = Convert.ToInt32(HttpContext.Session.GetString("user"));
            winkelmand.Aantal = Aantal;
            PC.DeleteWinkelmandItem(winkelmand);
            return RedirectToAction("Winkelmandje");
        }

        // Om naar de bevestiging te gaan.
        [Authorize]
        [HttpPost]
        public IActionResult Winkelmandje(VMbestelling vMbestelling)
        {
           
            return RedirectToAction("Bevestiging");
        }

        // Hier versturen we de bestelling en de mail.
        [Authorize]
        [HttpGet]
        public IActionResult Bevestiging(VMbestelling vMbestelling)
        {
            
            int klantnr = Convert.ToInt32(HttpContext.Session.GetString("user"));
            vMbestelling.klant = PC.loadKlant(klantnr);
            vMbestelling.Bestelling = PC.MaakBestelling(klantnr);
            double TotaalInclu = Convert.ToDouble(HttpContext.Session.GetString("TotaalInclu"));
            ViewBag.TotaalInclu = TotaalInclu;
            vMbestelling.Verzend(TotaalInclu);


            return View(vMbestelling);
        }
    }
}
