using ODP_NET_Theatre.DTO;
using ODP_NET_Theatre.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.DAO
{
    public interface IShowingDAO : ICRUDDao<Showing, int>
    {
		//metoda koja prikazuje sve predstave koje se prikazuju na odredjenoj sceni
		List<PlayStatsDTO> FindBySceneId(int idSc);

		//metoda koja racuna ukupan broj gledalaca, prosecan broj gledalaca i broj prikazivanja za svaku predstavu
		List<PlayShowingsStatsDTO> FindSumAvgCountForShowingPlay();

		//metoda koja prikazuje predstave ciji je broj gledalaca veci od broja sedista
		List<ShowingDeleteDTO> FindShowingForDeleting(IDbConnection connection);

		//metoda koja prikazuje sva prikazivanje odredjene predstave
		List<Showing> FindShowingByPlayId(int plId);

        //metoda koja vrsi jednu transakcionu obradu u zadatku 5
        List<ShowingDeleteDTO> DeleteAndInsertIntoShowing();
	}
}
