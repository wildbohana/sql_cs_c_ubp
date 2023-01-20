using ODP_NET_Theatre.DAO;
using ODP_NET_Theatre.DAO.Impl;
using ODP_NET_Theatre.DTO;
using ODP_NET_Theatre.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.Service
{
    public class ComplexFuncionalityService
    {
        private static readonly ITheatreDAO theatreDAO = new TheatreDAOImpl();
        private static readonly ISceneDAO sceneDAO = new SceneDAOImpl();
        private static readonly IShowingDAO showingDAO = new ShowingDAOImpl();
        private static readonly IPlayDAO playDAO = new PlayDAOImpl();
        private static readonly IRoleDAO roleDAO = new RoleDAOImpl();

        //complex query 1
        public List<ScenesForTheatreDTO> GetScenesForTheatres()
        {
            List<ScenesForTheatreDTO> ret = new List<ScenesForTheatreDTO>();

            foreach (Theatre t in theatreDAO.FindAll())
            {
                ScenesForTheatreDTO dto = new ScenesForTheatreDTO();
                dto.Theatre = t;
                dto.Scenes = sceneDAO.FindSceneByTheatre(t.IdTh);
                ret.Add(dto);
            }

            return ret;
        }

        //complex query 2
        public List<ShowingsForPlayDTO> GetShowingsForPlays()
        {
            List<ShowingsForPlayDTO> ret = new List<ShowingsForPlayDTO>();

            foreach (PlayShowingsStatsDTO stats in showingDAO.FindSumAvgCountForShowingPlay())
            {
                ShowingsForPlayDTO dto = new ShowingsForPlayDTO();
                dto.Stats = stats;
                dto.Play = playDAO.FindById(stats.PlayIdPl);
                dto.Showings = showingDAO.FindShowingByPlayId(stats.PlayIdPl);
                ret.Add(dto);
            }

            return ret;
        }

        //complex query 3
        public List<PlaysForSceneDTO> GetDataForComplexQuery()
        {
            List<PlaysForSceneDTO> result = new List<PlaysForSceneDTO>();

            foreach (Scene scene in sceneDAO.FindSceneForSpecificNumberOfSeats())
            {
                PlaysForSceneDTO dto = new PlaysForSceneDTO(scene);

                foreach (PlayStatsDTO play in showingDAO.FindBySceneId(scene.IdSc))
                {
                    play.RolesTotal = roleDAO.FindCountByPlayId(play.PlayId);
                    dto.Plays.Add(play);
                }
                result.Add(dto);
            }

            return result;
        }

        //complex query 4
        public List<PlayDTO> GetMostVisitedPlays()
        {
            List<PlayDTO> result = new List<PlayDTO>();

            foreach (PlayDTO playDTO in playDAO.FindMostVisitedPlays())
            {
                foreach (Role role in roleDAO.FindRoleByPlayId(playDTO.IdPl))
                {
                    playDTO.Roles.Add(role);
                }

                playDTO.MaleRolesTotal = roleDAO.FindCountForRoleGender(playDTO.IdPl, "m");
                playDTO.FemaleRolesTotal = roleDAO.FindCountForRoleGender(playDTO.IdPl, "z");

                result.Add(playDTO);
            }

            return result;
        }

        //complex query 5
        public List<ShowingDeleteDTO> DeleteShowings()
        {
            return showingDAO.DeleteAndInsertIntoShowing();
        }
    }
}