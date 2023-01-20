using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.UIHandler
{
    public class MainUIHandler
    {
        private readonly TheatreUIHandler pozoristeUIHandler = new TheatreUIHandler();
        private readonly SceneUIHandler sceneUIHandler = new SceneUIHandler();
        private readonly ComplexQueryUIHandler complexQueryUIHandler = new ComplexQueryUIHandler();

        public void HandleMainMenu()
        {
            string answer;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Odaberite opciju:");
                Console.WriteLine("1 - Rukovanje pozoristima");
                Console.WriteLine("2 - Rukovanje scenema");
                Console.WriteLine("3 - Kompleksni upiti");
                Console.WriteLine("X - Izlazak iz programa");

                answer = Console.ReadLine();

                switch (answer)
                {
                    case "1":
                        pozoristeUIHandler.HandleTheatreMenu();
                        break;
                    case "2":
                        sceneUIHandler.HandleSceneMenu();
                        break;
                    case "3":
                        complexQueryUIHandler.HandleComplexQueryMenu();
                        break;
                }
            } while (!answer.ToUpper().Equals("X"));
        }
    }
}
