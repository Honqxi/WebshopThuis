using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebshopGraphicsCard.Models
{
    public class Winkelmand
    {
        public int ArtNr { get; set; }
        public int Aantal { get; set; }
        public string Foto { get; set; }
        public string Naam { get; set; }
        public double Prijs { get; set; }
        
        public int KlantNr { get; set; }
    }
}
