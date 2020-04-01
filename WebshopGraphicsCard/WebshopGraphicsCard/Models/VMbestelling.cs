using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace WebshopGraphicsCard.Models
{
    public class VMbestelling
    {
        public Bestelling Bestelling { get; set; }
        public Klant klant { get; set; }

        
        public void Verzend(double totaalIncl)
        {
             
            string onderwerp = "Bestelling Online Videokaarten";
            string bericht = "Beste we hebben uw bestelling met het bestelnummer " + Bestelling.OrderNr + " goed ontvangen. \n" +
                             "Na de overschijving van € " + totaalIncl + " op het rekeningnummer BE88 9654 4123 6541, zal de verzending van u videokaarten beginnen.. \n" +
                             "Bedankt voor uw vertrouwen. \n" +
                             "VideoKaarten Online.";
            SendGridClient client = new SendGridClient(Environment.GetEnvironmentVariable("mijnAPIkey"));
            EmailAddress from = new EmailAddress("OnlineVideoKaartShop@gmail.com", "Online Videokaarten shop");
            EmailAddress to = new EmailAddress(klant.Mail, klant.Naam);
            client.SendEmailAsync(MailHelper.CreateSingleEmail(from, to, onderwerp, bericht, ""));                                                       
        }
    }
}
