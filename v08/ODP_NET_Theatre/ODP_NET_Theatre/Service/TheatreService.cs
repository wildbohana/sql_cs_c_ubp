using ODP_NET_Theatre.DAO;
using ODP_NET_Theatre.DAO.Impl;
using ODP_NET_Theatre.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.Service
{
    public class TheatreService
    {
        private static readonly ITheatreDAO theatreDAO = new TheatreDAOImpl();

        public List<Theatre> FindAll()
        {
            return theatreDAO.FindAll().ToList();
        }

        public Theatre FindById(int id)
        {
            return theatreDAO.FindById(id);
        }

        public int Save(Theatre p)
        {
            return theatreDAO.Save(p);
        }

        public bool ExistsById(int id)
        {
            return theatreDAO.ExistsById(id);
        }

        public int DeleteById(int id)
        {
            return theatreDAO.DeleteById(id);
        }
        public int SaveAll(List<Theatre> pozoristeList)
        {
            return theatreDAO.SaveAll(pozoristeList);
        }
    }
}
