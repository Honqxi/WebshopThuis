using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebshopGraphicsCard.Models
{
    public class Artikel
    {
        public int ArtNr { get; set; }
        public string Naam { get; set; }
        public int Voorraad { get; set; }
        public double Prijs { get; set; }
        public string Foto { get; set; } 
    }
}
