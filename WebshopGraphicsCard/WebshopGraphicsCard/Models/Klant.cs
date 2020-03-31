using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebshopGraphicsCard.Models
{
    public class Klant
    {
        public int KlantNr { get; set; }
        public string Naam { get; set; }
        public string Voornaam { get; set; }
        public string Adres { get; set; }
        public string PostCode { get; set; }
        public string Gemeente { get; set; }
        public string Mail { get; set; }

        [Required(ErrorMessage ="Verplicht Veld.")]
        public string Gebruikersnaam { get; set; }
        [Required(ErrorMessage = "Verplicht Veld.")]
        public string Wachtwoord { get; set; }
    }
}
