using ODP_NET_Theatre.DTO;
using ODP_NET_Theatre.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.DAO
{
    public interface IPlayDAO : ICRUDDao<Play, int>
    {
        //metoda koja vraca najposecenije predstave (moze ih biti vise jednako posecenih zbog toga je lista, a ne jedna predstava)
        List<PlayDTO> FindMostVisitedPlays();
    }
}
