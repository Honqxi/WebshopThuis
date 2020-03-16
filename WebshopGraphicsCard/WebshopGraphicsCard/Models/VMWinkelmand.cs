using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebshopGraphicsCard.Models
{
    public class VMWinkelmand
    {
        
        public WinkelmandRepository WinkelmandRepo { get; set; }
        public Klant Klant { get; set; }
        public BerekendGegeven BerekendGegeven { get; set; }

        

    }
}
