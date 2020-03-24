using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebshopGraphicsCard.Models;
using Microsoft.AspNetCore.Http;

namespace WebshopGraphicsCard.Persistence
{
    public class PersistenceCode
    {
        //We verbinden onze databank met ons project.
        string ConnStr = "server=localhost;user id=root;Password=Test123;database=dbwebshop";

        //Hier halen we alle artikels op uit onze databank.
        public List<Artikel> loadArtikels()
        {
            MySqlConnection conn = new MySqlConnection(ConnStr);
            conn.Open();
            string qry = "select * from tblartikel";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            List<Artikel> lijst = new List<Artikel>();
            while (dtr.Read())
            {
                Artikel artikel = new Artikel();
                artikel.ArtNr = Convert.ToInt32(dtr["ArtNr"]);
                artikel.Naam = Convert.ToString(dtr["Naam"]);
                artikel.Voorraad = Convert.ToInt32(dtr["Voorraad"]);
                artikel.Prijs = Convert.ToDouble(dtr["Prijs"]);
                artikel.Foto = Convert.ToString(dtr["Foto"]);

                lijst.Add(artikel);
            }
            conn.Close();
            return lijst;
        }
        
        public Artikel loadArtikel(int Nr)
        {
            MySqlConnection conn = new MySqlConnection(ConnStr);
            conn.Open();
            string qry = "select * from tblartikel where ArtNr='" + Nr + "'";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            Artikel artikel = new Artikel();
            while (dtr.Read())
            {
                
                artikel.ArtNr = Convert.ToInt32(dtr["ArtNr"]);
                artikel.Naam = Convert.ToString(dtr["Naam"]);
                artikel.Voorraad = Convert.ToInt32(dtr["Voorraad"]);
                artikel.Prijs = Convert.ToDouble(dtr["Prijs"]);
                artikel.Foto = Convert.ToString(dtr["Foto"]);

                
            }
            conn.Close();
            return artikel;
        }


        public void PasMandjeAan(Winkelmand winkelmand)
        {
            MySqlConnection conn1 = new MySqlConnection(ConnStr);
            conn1.Open();

            //Hier controleer je of het artikel al in het winkel mandje zit
            string qry1 = "Select * from tblwinkelmand where (Artnr = '" + winkelmand.ArtNr + "') and (KlantNr = " + winkelmand.KlantNr + ")";                   
            MySqlCommand cmd1 = new MySqlCommand(qry1, conn1);
            MySqlDataReader dtr1 = cmd1.ExecuteReader();
            

            //zoja, dan update je het aantal
            if (dtr1.HasRows)
            {
                conn1.Close();
                MySqlConnection conn2 = new MySqlConnection(ConnStr);
                conn2.Open();
                //Het huidige aantal in het winkelmandje aanpassen met de nieuwe hoeveelheid
                string qry2 = "update tblwinkelmand set aantal = (aantal +" + winkelmand.Aantal + ") where (Artnr = '" + winkelmand.ArtNr + "') and (KlantNr = '" + winkelmand.KlantNr + "')";
                MySqlCommand cmd2 = new MySqlCommand(qry2, conn2);
                cmd2.ExecuteNonQuery();
                conn2.Close();
                
            }
            else // Zonee, dan insert je het artikel en het aantal
            {
                MySqlConnection conn3 = new MySqlConnection(ConnStr);
                conn3.Open();
                //Als het artikel niet in het winkelmandje zit dan voeg je het eraan toe 
                string qry3 = "INSERT INTO `tblwinkelmand` (`KlantNr`, `ArtNr`, `Aantal`) VALUES ('"+ winkelmand.KlantNr+"', '" + winkelmand.ArtNr + "', '" + winkelmand.Aantal + "')";
                MySqlCommand cmd3 = new MySqlCommand(qry3, conn3);
                 cmd3.ExecuteNonQuery();
                conn3.Close();
            }

            //Huidige voorraad aanpassen (Aantal dat naar het winkelmandje gaat aftrekken van de huidige voorraad)
            MySqlConnection conn4 = new MySqlConnection(ConnStr);
            conn4.Open();
            string qry4 = "update tblartikel set voorraad = voorraad - '" + winkelmand.Aantal + "' where (Artnr = '" + winkelmand.ArtNr + "')";
            MySqlCommand cmd4 = new MySqlCommand(qry4, conn4);
            cmd4.ExecuteNonQuery();
            conn4.Close();
        }

        //De klant zijn gegevens opladen, dit hebben we nodig in de view van winkelmandje en bestelling.
        public Klant loadKlant(int KlantNr)
        {

            MySqlConnection conn = new MySqlConnection(ConnStr);
            conn.Open();
            string qry = "select * from tblklant where KlantNr='" +  KlantNr+"'";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            Klant klant = new Klant();
            while (dtr.Read())
            {
                
                klant.KlantNr = KlantNr;
                klant.Naam = Convert.ToString(dtr["Naam"]);
                klant.Voornaam = Convert.ToString(dtr["Voornaam"]);
                klant.Adres = Convert.ToString(dtr["Adres"]);
                klant.PostCode = Convert.ToString(dtr["PostCode"]);
                klant.Gemeente = Convert.ToString(dtr["Gemeente"]);
                klant.Mail = Convert.ToString(dtr["Mail"]);
                

            }
            conn.Close();
            return klant;

        }

        //Het inladen van de lijst van artikels die in het winkelmandje zitten. (Ophalen uit de databank.)
        public List<Winkelmand> loadWinkelmand(int KlantNr)
        {
            MySqlConnection conn = new MySqlConnection(ConnStr);
            conn.Open();
            //string qry = "select (prijs*aantal) as totaalexcl, tblwinkelmand.KlantNr, tblwinkelmand.ArtNr, tblwinkelmand.Aantal, tblartikel.Foto, tblartikel.Naam, tblartikel.Prijs from tblwinkelmand inner join tblartikel on tblwinkelmand.ArtNr = tblartikel.ArtNr where KlantNr = '1'";

            string qry = "select (prijs*aantal) as totaalexcl, tblwinkelmand.KlantNr, tblwinkelmand.ArtNr, tblwinkelmand.Aantal, tblartikel.Foto, tblartikel.Naam, tblartikel.Prijs from tblwinkelmand inner join tblartikel on tblwinkelmand.ArtNr = tblartikel.ArtNr where KlantNr = '" + KlantNr + "'";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            List<Winkelmand> lijst = new List<Winkelmand>();
            while (dtr.Read())
            {
                Winkelmand winkelmand = new Winkelmand();
                winkelmand.ArtNr = Convert.ToInt32(dtr["ArtNr"]);
                winkelmand.Aantal = Convert.ToInt32(dtr["Aantal"]);
                winkelmand.KlantNr = Convert.ToInt32(dtr["KlantNr"]);
                winkelmand.Naam = Convert.ToString(dtr["Naam"]);
                winkelmand.Prijs = Convert.ToDouble(dtr["Prijs"]);
                winkelmand.Foto = Convert.ToString(dtr["Foto"]);

                lijst.Add(winkelmand);
            }
            conn.Close();
            return lijst;
        }



        public BerekendGegeven BerekenTotaal()
        {
            MySqlConnection conn = new MySqlConnection(ConnStr);
            conn.Open();
            string qry = "select sum((prijs * aantal)) as TotaalExclu, sum(((prijs * aantal)*0.21)) as btw, sum( ((prijs * aantal)*1.21)) as TotaalInclu from tblartikel inner join tblwinkelmand on tblwinkelmand.ArtNr = tblArtikel.ArtNr";
;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            BerekendGegeven berekendGegeven = new BerekendGegeven();

            while (dtr.Read())
            {
                berekendGegeven.BTW = Math.Round( Convert.ToDouble(dtr["BTW"]),2);
                berekendGegeven.TotaalInclu = Math.Round( Convert.ToDouble(dtr["TotaalInclu"]),2);
                berekendGegeven.TotaalExclu = Math.Round(Convert.ToDouble(dtr["TotaalExclu"]), 2);               
            }
            conn.Close();
            return berekendGegeven;
        }



        // Het item terug toevoegen in de voorraad van het artikel. Daarna het artikel zelf verwijderen uit het winkelmandje.
        public void DeleteWinkelmandItem( Winkelmand winkelmand)
        {
            MySqlConnection conn = new MySqlConnection(ConnStr);
            conn.Open();
            string qry = "update tblartikel set voorraad = voorraad+" + winkelmand.Aantal + " where ArtNr=" + winkelmand.ArtNr;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            cmd.ExecuteNonQuery();
            conn.Close();

            MySqlConnection conn2 = new MySqlConnection(ConnStr);
            conn2.Open();
            string qry2 = "DELETE FROM `tblwinkelmand` WHERE(`KlantNr` = '" + winkelmand.KlantNr +"') and (`ArtNr` = '" + winkelmand.ArtNr + "')";
            MySqlCommand cmd2 = new MySqlCommand(qry2, conn2);
            cmd2.ExecuteNonQuery();
            conn2.Close();        
        }


        //Het Controleren van het winkelmandje. Heeft deze items voor de klant of niet

        public bool ControleerWinkelmand(int klantnr)
        {
            MySqlConnection conn = new MySqlConnection(ConnStr);
            conn.Open();
            string qry = "select * from tblwinkelmand where klantnr =" + klantnr;
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            MySqlDataReader dtr = cmd.ExecuteReader();
            if (dtr.HasRows == true)
            {
                conn.Close();
                return true;
            }
            else
            {
                conn.Close();
                return false;
            }
        }


        public Bestelling MaakBestelling(int KlantNr)
        {
            //bestelling aanmaken. Alle gegevens invullen in de tabel bestelling.
            MySqlConnection conn = new MySqlConnection(ConnStr);
            conn.Open();
            Bestelling bestelling = new Bestelling();
            bestelling.Datum = DateTime.Now;
            string CorrDatum = bestelling.Datum.ToString("yyyy-MM-dd");
            string qry = "insert into tblBestelling (datum, KlantNr) VALUES ('" + CorrDatum + ", " + KlantNr + ")";
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            cmd.ExecuteNonQuery();
            conn.Close();

            //OrderNr uit tblbestelling halen, Om die later terug in te plaatsen.
            MySqlConnection conn2 = new MySqlConnection(ConnStr);
            conn2.Open();
            string qry2 = "select OrderNr from tblbestelling where KlantNr= " + KlantNr + "order by OrderNr desc LIMIT 1";
            MySqlCommand cmd2 = new MySqlCommand();
            MySqlDataReader dtr2 = cmd2.ExecuteReader();
            while (dtr2.Read())
            {
                bestelling.OrderNr = Convert.ToInt32(dtr2["OrderNr"]);
            }
            conn2.Close();

            //Alles uit het winkelmandje halen.
            MySqlConnection conn3 = new MySqlConnection(ConnStr);
            conn3.Open();
            string qry3 = "select tblwinkelmand.ArtNr, Prijs, tblwinkelmand.Aantal from tblwinkelmand inner join tblArtikel on tblArtikel.artnr = tblwinkelmand.artnr where tblwinkelmand.KlantNr = '" + KlantNr + "'";
            MySqlCommand cmd3 = new MySqlCommand(qry, conn);
            MySqlDataReader dtr3 = cmd3.ExecuteReader();
            List<Winkelmand> lijst = new List<Winkelmand>();

            while (dtr3.Read())
            {
                Winkelmand winkelmand = new Winkelmand();
                winkelmand.Prijs = Convert.ToDouble(dtr3["Prijs"]);
                winkelmand.ArtNr = Convert.ToInt32(dtr3["ArtNr"]);
                winkelmand.Aantal = Convert.ToInt32(dtr3["Aantal"]);

                lijst.Add(winkelmand);
            }
            conn3.Close();

            //Bestellijn creeëren, alle opgehaalde items terug in de bestellijn steken.
           
            foreach (var item in lijst)
            {
                MySqlConnection conn4 = new MySqlConnection(ConnStr);
                conn4.Open();
                string corrPrijs = Convert.ToString(item.Prijs).Replace(",", ".");
                string qry4 = "insert into tblBestellijn (OrderNr, ArtNr,Aantal,Prijs) VALUES (" + bestelling.OrderNr + "," + item.ArtNr + "," + item.Aantal + "," + corrPrijs + ")";
                MySqlCommand cmd4 = new MySqlCommand(qry4, conn4);
                cmd4.ExecuteNonQuery();
                conn4.Close();
            }

            return bestelling;
        }



    }
}
