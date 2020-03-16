using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebshopGraphicsCard.Models
{
    public class VMToevoegen
    {
        public Artikel artikel { get; set; }
        [Required(ErrorMessage ="Verplicht in te vullen veld")]
        public int Aantal { get; set; }
    }
}
