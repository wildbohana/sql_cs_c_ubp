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
    public class SceneUIHandler
    {
        private static readonly SceneService sceneService = new SceneService();

        public void HandleSceneMenu()
        {
            string answer;

            do
            {
                Console.WriteLine();
                Console.WriteLine("Odaberite opciju za rad nad scenema:");
                Console.WriteLine("1 - Prikaz svih");
                Console.WriteLine("2 - Prikaz po identifikatoru");
                Console.WriteLine("X - Izlazak iz rukovanja scenema");

                answer = Console.ReadLine();

                switch (answer)
                {
                    case "1":
                        ShowAll();
                        break;
                    case "2":
                        ShowById();
                        break;
                }
            } while (!answer.ToUpper().Equals("X"));
        }

        private void ShowAll()
        {
            Console.WriteLine(Scene.GetFormattedHeader());

            try
            {
                foreach (Scene scene in sceneService.FindAll())
                {
                    Console.WriteLine(scene);
                }
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void ShowById()
        {
            Console.WriteLine("IDSCE: ");
            int id = int.Parse(Console.ReadLine());

            try
            {
                Scene scene = sceneService.FindById(id);

                Console.WriteLine(Scene.GetFormattedHeader());
                Console.WriteLine(scene);
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
