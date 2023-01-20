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
    public class SceneService
    {
        private static readonly ISceneDAO sceneDAO = new SceneDAOImpl();

        public List<Scene> FindAll()
        {
            return sceneDAO.FindAll().ToList();
        }

        public Scene FindById(int id)
        {
            return sceneDAO.FindById(id);
        }

        public List<Scene> FindSceneByTheatre(int id)
        {
		    return sceneDAO.FindSceneByTheatre(id);
        }
    }
}
