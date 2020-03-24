using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebshopGraphicsCard.Models
{
    public class Bestelling
    {
        public int OrderNr { get; set; }
        public int KlantNr { get; set; }
        public DateTime Datum { get; set; }
    }  
}
