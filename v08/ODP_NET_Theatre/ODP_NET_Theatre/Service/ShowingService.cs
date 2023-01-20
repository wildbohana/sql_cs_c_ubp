using ODP_NET_Theatre.DAO;
using ODP_NET_Theatre.DAO.Impl;
using ODP_NET_Theatre.DTO;
using ODP_NET_Theatre.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.Service
{
    public class ShowingService
    {
		private static readonly IShowingDAO showingDAO = new ShowingDAOImpl();

		public List<PlayStatsDTO> GetBySceneId(int id)
		{
			return showingDAO.FindBySceneId(id);
		}

		public List<Showing> GetAll()
		{
			return showingDAO.FindAll().ToList();
		}

		public List<Showing> GetAllShowingForOnePlay(int id)
		{
			return showingDAO.FindShowingByPlayId(id);
		}
	}
}
