using ODP_NET_Theatre.DTO;
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
    public class ComplexQueryUIHandler
    {
        private static readonly ComplexFuncionalityService complexQueryService = new ComplexFuncionalityService();
        private static readonly TheatreService theatreService = new TheatreService();
        private static readonly SceneService sceneService = new SceneService();
        private static readonly ShowingService showingService = new ShowingService();
        private static readonly PlayService playService = new PlayService();

        public void HandleComplexQueryMenu()
        {
            String answer;

            do
            {
                Console.WriteLine("\nOdaberite funkcionalnost:");
                Console.WriteLine(
                    "\n1  - Za svako pozoriste prikazati listu scene koje ima. Ukoliko pozoriste nema scenu ispisati: NEMA SCENE");
                Console.WriteLine(
                    "\n2  - Prikazati informacije o predstavama koje se prikazuju. Pored osnovnih informacija o predstavama prikazati sva prikazivanja" +
                    "\n     za svaku od njih. Za svaku predstavu prikazati ukupan broj gledalaca, prosecan broj gledalaca i broj prikazivanja.");
                Console.WriteLine(
                    "\n3  - Prikazati nazive scene i broj sedista za sve scene ciji broj sedista je u intervalu plus/minus 20% od broja sedista koje ima scene Joakim Vujic " +
                    "\n     pozorista Knjazevsko-srpski teatar Kragujevac. Za sve predstave koje se prikazuju na tim scenema izracunati ukupan broj gledalaca." +
                    "\n     Prikazati samo one predstave ciji je ukupan broj gledalaca veci od 300. Za te predstave prikazati ukupan broj uloga na toj predstavi. " +
                    "\n     Za scene na kojima se ne prikazuju predstave ispisati poruku \"NEMA PREDSTAVA ZA PRIKAZIVANJE NA OVOJ SCENI!\"");
                Console.WriteLine(
                    "\n4  - Prikazati id, nazive i prosecan broj gledalaca predstava koje imaju najveci prosecan broj gledalaca. Za te predstave prikazati listu uloga." +
                    "\n     Pored toga prikazati koliko ukupno ima muskih uloga i koliko ukupno ima zenskih uloga..");
                Console.WriteLine(
                    "\n5  - Prikazati prikazivanja predstava u narednom periodu na scenema na kojima je broj gledalaca veci od broja sedista na toj sceni. " +
                    "\n     Obrisati te torke. Raspodeliti sedista tako da se uvedu novi termini prikazivanja ove predstave na toj sceni. " +
                    "\n     Zauzeti pune scene onoliko puta koliko je potrebno, i napuniti poslednju scenu sa onoliko mesta koliko je ostalo. " +
                    "\n     Za novi datum uneti danasnji datum. Ispisati sva prikazivanja.");
                Console.WriteLine("\nX  - Izlazak iz kompleksnih upita");

                answer = Console.ReadLine();

                switch (answer)
                {
                    case "1":
                        ShowSceneForTheatre();
                        break;
                    case "2":
                        ShowReportingForShowingShows();
                        break;
                    case "3":
                        ShowComplexQuery();
                        break;
                    case "4":
                        ShowMostVisitedShow();
                        break;
                    case "5":
                        ShowShowingForDeleting();
                        break;
                }
            } while (!answer.ToUpper().Equals("X"));
        }

        private void ShowSceneForTheatre()
        {
            try
            {
                List<ScenesForTheatreDTO> dtos = complexQueryService.GetScenesForTheatres();
                if (dtos.Count != 0)
                {
                    foreach (ScenesForTheatreDTO dto in dtos)
                    {
                        Console.WriteLine(dto.Theatre);
                        Console.WriteLine("\t\t------------------------------- SCENE -------------------------------");

                        if (dto.Scenes.Count != 0)
                        {
                            Console.WriteLine("\t\t" + Scene.GetFormattedHeader());
                            foreach (Scene scene in dto.Scenes)
                            {
                                Console.WriteLine("\t\t" + scene);
                            }
                        }
                        else
                        {
                            Console.WriteLine("\t\tNEMA SCENE");
                        }
                        Console.WriteLine("\t\t---------------------------------------------------------------------");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("\t\tNEMA POZORISTA.");
                }
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ShowReportingForShowingShows()
        {
            try
            {
                foreach (ShowingsForPlayDTO dto in complexQueryService.GetShowingsForPlays()) 
                {
				    String statsHeader =  String.Format("{0,-30} {1,-30:F2} {2,-30}", "UKUPAN BROJ GLEDALACA", "PROSECAN BROJ GLEDALACA", "BROJ PRIKAZIVANJA");
                    Console.WriteLine(Play.GetFormattedHeader() + " " + statsHeader);
                    Console.WriteLine(dto.Play + " " + dto.Stats);

                    Console.WriteLine("\t\t---------------------------PRIKAZIVANJA----------------------------------");
                    Console.WriteLine("\t\t" + Showing.GetFormattedHeader());
				    
                    foreach(Showing showing in dto.Showings) 
                    {
                        Console.WriteLine("\t\t" + showing);
				    }
                    Console.WriteLine("\t\t-------------------------------------------------------------------------");
                    Console.WriteLine("\n\n");
			    }
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ShowComplexQuery()
        {
            try
            {
                Console.WriteLine(Scene.GetFormattedHeader());

                foreach (PlaysForSceneDTO dto in complexQueryService.GetDataForComplexQuery())
                {
                    Console.WriteLine(dto.Scene);
                    
                    if (dto.Plays.Count() != 0)
                    {
                        Console.WriteLine("       Predstave:");

                        foreach (PlayStatsDTO showing in dto.Plays)
                        {
                            if (showing.SpectatorsTotal > 300)
                            {
                                Console.WriteLine("\t\t* " + Play.GetFormattedHeader());
                                Console.WriteLine("\t\t  " + playService.GetById(showing.PlayId));
                                Console.WriteLine("\t\t  Ukupna suma broja gledalaca: " + showing.SpectatorsTotal);
                                Console.WriteLine("\t\t  Ukupan broj uloga za predstavu: " + showing.RolesTotal);

                            }
                            else
                            {
                                Console.WriteLine("\t\t* (Preskocena jedna predstava za prikazivanje na ovoj sceni jer nema više od 300 gledalaca)");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("\t\t* NEMA PREDSTAVA ZA PRIKAZIVANJE NA OVOJ SCENI!");
                    }

                    Console.WriteLine();
                }
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ShowMostVisitedShow()
        {
            try
            {
                foreach (PlayDTO p in complexQueryService.GetMostVisitedPlays())
                {
                    Console.WriteLine(PlayDTO.GetFormatedHeader());
                    Console.WriteLine(p);

                    Console.WriteLine("\t\t--------------------ULOGE------------------------");
                    Console.WriteLine("\t\t" + Role.GetFormattedHeader());
                    foreach (Role role in p.Roles)
                    {
                        Console.WriteLine("\t\t" + role);
                    }
                    
                    Console.WriteLine("\t\t-----------UKUPAN BROJ ZENSKIH ULOGA-------------");
                    Console.WriteLine("\t\t" + p.FemaleRolesTotal);

                    Console.WriteLine("\t\t-----------UKUPAN BROJ MUSKIH ULOGA--------------");
                    Console.WriteLine("\t\t" + p.MaleRolesTotal);
                }
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ShowShowingForDeleting()
        {
            try
            {
                List<ShowingDeleteDTO> forDeleting = complexQueryService.DeleteShowings();

                if (forDeleting.Count == 0)
                {
                    Console.WriteLine("Nema prikazivanja za brisanje.");
                }
                else
                {
                    foreach (ShowingDeleteDTO pd in forDeleting)
                    {
                        Console.WriteLine(ShowingDeleteDTO.GetFormattedHeader());
                        Console.WriteLine(pd);
                    }
                    Console.WriteLine("--------------------Nakon brisanja:---------------------");

                    Console.WriteLine(Showing.GetFormattedHeader());
                    foreach (Showing p in showingService.GetAll())
                    {
                        Console.WriteLine(p);
                    }
                }
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
