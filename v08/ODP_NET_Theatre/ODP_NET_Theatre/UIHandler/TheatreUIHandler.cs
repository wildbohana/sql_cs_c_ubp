using ODP_NET_Theatre.DAO;
using ODP_NET_Theatre.DAO.Impl;
using ODP_NET_Theatre.Model;
using ODP_NET_Theatre.Service;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.UIHandler
{
    public class TheatreUIHandler
    {
        private static readonly TheatreService theatreService = new TheatreService();

        public void HandleTheatreMenu()
        {
            String answer;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Odaberite opciju za rad nad pozoristima:");
                Console.WriteLine("1 - Prikaz svih");
                Console.WriteLine("2 - Prikaz po identifikatoru");
                Console.WriteLine("3 - Unos jednog pozorista");
                Console.WriteLine("4 - Unos vise pozorista");
                Console.WriteLine("5 - Izmena po identifikatoru");
                Console.WriteLine("6 - Brisanje po identifikatoru");
                Console.WriteLine("X - Izlazak iz rukovanja pozoristima");

                answer = Console.ReadLine();

                switch (answer)
                {
                    case "1":
                        ShowAll();
                        break;
                    case "2":
                        ShowById();
                        break;
                    case "3":
                        HandleSingleInsert();
                        break;
                    case "4":
                        HandleMultipleInserts();
                        break;
                    case "5":
                        HandleUpdate();
                        break;
                    case "6":
                        HandleDelete();
                        break;
                }
            } while (!answer.ToUpper().Equals("X"));
        }

        private void ShowAll()
        {
            Console.WriteLine(Theatre.GetFormattedHeader());

            try
            {
                foreach (Theatre theatre in theatreService.FindAll())
                {
                    Console.WriteLine(theatre);
                }
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ShowById()
        {
            Console.WriteLine("IDPOZ: ");
            int id = int.Parse(Console.ReadLine());

            try
            {
                Theatre theatre = theatreService.FindById(id);

                Console.WriteLine(Theatre.GetFormattedHeader());
                Console.WriteLine(theatre);
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void HandleSingleInsert()
        {
            Console.WriteLine("IDPOZ: ");
            int id = int.Parse(Console.ReadLine());

            Console.WriteLine("Naziv: ");
            string nameTh = Console.ReadLine();

            Console.WriteLine("Adresa: ");
            string addressTh = Console.ReadLine();

            Console.WriteLine("Sajt: ");
            string webisteTh = Console.ReadLine();

            Console.WriteLine("Mesto: ");
            int placeIdPl = int.Parse(Console.ReadLine());

            try
            {
                int inserted = theatreService.Save(new Theatre(id, nameTh, addressTh, webisteTh, placeIdPl));
                if (inserted != 0)
                {
                    Console.WriteLine("Pozoriste \"{0}\" uspešno uneto.", nameTh);
                }
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void HandleUpdate()
        {
            Console.WriteLine("IDPOZ: ");
            int id = int.Parse(Console.ReadLine());

            try
            {
                if (!theatreService.ExistsById(id))
                {
                    Console.WriteLine("Uneta vrednost ne postoji!");
                    return;
                }

                Console.WriteLine("Naziv: ");
                string nameTh = Console.ReadLine();

                Console.WriteLine("Adresa: ");
                string addressTh = Console.ReadLine();

                Console.WriteLine("Sajt: ");
                string webisteTh = Console.ReadLine();

                Console.WriteLine("Mesto: ");
                int placeIdPl = int.Parse(Console.ReadLine());

                int updated = theatreService.Save(new Theatre(id, nameTh, addressTh, webisteTh, placeIdPl));
                if (updated != 0)
                {
                    Console.WriteLine("Pozoriste \"{0}\" uspešno izmenjeno.", id);
                }
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void HandleDelete()
        {
            Console.WriteLine("IDPOZ: ");
            int id = int.Parse(Console.ReadLine());

            try
            {
                int deleted = theatreService.DeleteById(id);
                if (deleted != 0)
                {
                    Console.WriteLine("Pozoriste sa šifrom \"{0}\" uspešno obrisano.", id);
                }
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void HandleMultipleInserts()
        {
            List<Theatre> theatreList = new List<Theatre>();
            String answer;

            do
            {
                Console.WriteLine("IDPOZ: ");
                int id = int.Parse(Console.ReadLine());

                Console.WriteLine("Naziv: ");
                string nameTh = Console.ReadLine();

                Console.WriteLine("Adresa: ");
                string addressTh = Console.ReadLine();

                Console.WriteLine("Sajt: ");
                string webisteTh = Console.ReadLine();

                Console.WriteLine("Mesto: ");
                int placeIdPl = int.Parse(Console.ReadLine());

                theatreList.Add(new Theatre(id, nameTh, addressTh, webisteTh, placeIdPl));

                Console.WriteLine("Unesi još jedno pozorište? (ENTER za potvrdu, X za odustanak)");
                answer = Console.ReadLine();
            } while (!answer.ToUpper().Equals("X"));

            try
            {
                int numInserted = theatreService.SaveAll(theatreList);
                Console.WriteLine("Uspešno uneto {0} pozorišta.", numInserted);
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
