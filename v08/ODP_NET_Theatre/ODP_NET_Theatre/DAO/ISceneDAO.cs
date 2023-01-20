using ODP_NET_Theatre.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.DAO
{
    public interface ISceneDAO : ICRUDDao<Scene, int>
    {
        // metoda koja vraca sve scene za odredjeno pozoriste
        List<Scene> FindSceneByTheatre(int theatreId);

        // metoda za 2 zadatak iz ComplexUIHandler
        List<Scene> FindSceneForSpecificNumberOfSeats();
    }
}
