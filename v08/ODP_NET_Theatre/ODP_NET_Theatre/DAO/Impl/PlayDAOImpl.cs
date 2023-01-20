using ODP_NET_Theatre.ConnectionPool;
using ODP_NET_Theatre.DTO;
using ODP_NET_Theatre.Model;
using ODP_NET_Theatre.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.DAO.Impl
{
    public class PlayDAOImpl : IPlayDAO
    {
        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Delete(Play entity)
        {
            throw new NotImplementedException();
        }

        public int DeleteAll()
        {
            throw new NotImplementedException();
        }

        public int DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public bool ExistsById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Play> FindAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Play> FindAllById(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public Play FindById(int id)
        {
            Play play = null;
            string query = "" +
                "select id_pl, name_pl, duration_pl, year_pl " +
                "from play where id_pl = :id_pl";

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    ParameterUtil.AddParameter(command, "id_pl", DbType.Int32);                    
                    command.Prepare();

                    ParameterUtil.SetParameterValue(command, "id_pl", id);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            play = new Play(reader.GetInt32(0), reader.GetString(1),
                                reader.GetString(2), reader.GetInt32(3));
                        }
                    }
                }
            }
            return play;
        }

        public List<PlayDTO> FindMostVisitedPlays()
        {
            List<PlayDTO> result = new List<PlayDTO>();
            String query = "" +
                "SELECT p.id_pl ,name_pl, AVG(s.numofspec_sh)\n " +
                "FROM play p, showing s \n " +
                "WHERE p.id_pl = s.play_id_pl\n " + 
                "GROUP BY p.id_pl, name_pl \n " +
                "HAVING AVG(s.numofspec_sh) >= ALL" +
                "(SELECT AVG(numofspec_sh) FROM showing GROUP BY play_id_pl)";
    
            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Prepare();

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PlayDTO play = new PlayDTO(reader.GetInt32(0), reader.GetString(1), reader.GetDouble(2));
                            result.Add(play);
                        }
                    }
                }
            }
            return result;
        }

        public int Save(Play entity)
        {
            throw new NotImplementedException();
        }

        public int SaveAll(IEnumerable<Play> entities)
        {
            throw new NotImplementedException();
        }
    }
}
