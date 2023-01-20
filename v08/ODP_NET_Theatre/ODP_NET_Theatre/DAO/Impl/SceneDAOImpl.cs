using ODP_NET_Theatre.ConnectionPool;
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
    public class SceneDAOImpl : ISceneDAO
    {
        public int Count()
        {
            string query = "select count(*) from scene";

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Prepare();

                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public int Delete(Scene entity)
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

        public IEnumerable<Scene> FindAll()
        {
            string query = "select id_sc, name_sc, numofseats_sc, theatre_id_th from scene";
            List<Scene> sceneList = new List<Scene>();

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
                            Scene scene = new Scene(reader.GetInt32(0), reader.GetString(1),
                                reader.GetInt32(2), reader.GetInt32(3));
                            sceneList.Add(scene);
                        }
                    }
                }
            }
            return sceneList;
        }

        public IEnumerable<Scene> FindAllById(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public Scene FindById(int id)
        {
            Scene scene = null;
            string query = "" +
                "select id_sc, name_sc, numofseats_sc, theatre_id_th " +
                "from scene where id_sc = :id_sc";

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    ParameterUtil.AddParameter(command, "id_sc", DbType.Int32);
                    command.Prepare();

                    ParameterUtil.SetParameterValue(command, "id_sc", id);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            scene = new Scene(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3));
                        }
                    }
                }
            }
            return scene;
        }

        public List<Scene> FindSceneByTheatre(int theatreId)
        {
            List<Scene> sceneList = new List<Scene>();
            string query = "" + 
                "select id_sc, name_sc, numofseats_sc, theatre_id_th " +
                "from scene " +
                "where theatre_id_th = :theatre_id_th";

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    ParameterUtil.AddParameter(command, "theatre_id_th", DbType.Int32);
                    command.Prepare();

                    ParameterUtil.SetParameterValue(command, "theatre_id_th", theatreId);
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Scene scene = new Scene(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3));
                            sceneList.Add(scene);
                        }
                    }
                }
            }
            return sceneList;
        }

        public List<Scene> FindSceneForSpecificNumberOfSeats()
        {
            String query = "" +
                "SELECT S.id_sc,S.name_sc, s.numofseats_sc, s.theatre_id_th \n" +
                "FROM Scene S,  Scene S1, Theatre T1 \n" +
                "WHERE s.numofseats_sc >= s1.numofseats_sc*0.8 AND \n" +
                "s.numofseats_sc <= s1.numofseats_sc*1.2 AND " +
                "s1.name_sc = 'Scena Joakim Vujic' AND s1.theatre_id_th = t1.id_th AND \n" +
                "t1.name_th = 'Knjazevsko-srpski teatar Kragujevac'";
            List<Scene> sceneList = new List<Scene>();

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
                            Scene scene = new Scene(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3));
                            sceneList.Add(scene);
                        }
                    }
                }
            }
            return sceneList;
        }

        public int Save(Scene entity)
        {
            throw new NotImplementedException();
        }

        public int SaveAll(IEnumerable<Scene> entities)
        {
            throw new NotImplementedException();
        }
    }
}
