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
    public class PlayService
    {
        private static readonly IPlayDAO playDAO = new PlayDAOImpl();

        public Play GetById(int id)
        {
            return playDAO.FindById(id);
        }
    }
}
