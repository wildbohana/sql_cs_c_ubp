using ODP_NET_Theatre.ConnectionPool;
using ODP_NET_Theatre.DTO;
using ODP_NET_Theatre.Model;
using ODP_NET_Theatre.Utils;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.DAO.Impl
{
    public class ShowingDAOImpl : IShowingDAO
    {
        public int Count()
        {
            string query = "select count(*) from showing";

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

        public int Delete(Showing entity)
        {
            throw new NotImplementedException();
        }

        public int DeleteAll()
        {
            throw new NotImplementedException();
        }
        
        public int DeleteById(int id)
        {
            string query = "delete from showing where ordnum_sh=:ordnum_sh";

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    ParameterUtil.AddParameter(command, "ordnum_sh", DbType.Int32);
                    command.Prepare();

                    ParameterUtil.SetParameterValue(command, "ordnum_sh", id);

                    return command.ExecuteNonQuery();
                }
            }
        }

        public bool ExistsById(int id)
        {
            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                return ExistsById(id, connection);
            }
        }

        // connection set as parameter because this method is used in a transaction (see saveAll method)
        public bool ExistsById(int id, IDbConnection connection)
        {
            string query = "select * from showing where ordnum_sh=:ordnum_sh";

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                ParameterUtil.AddParameter(command, "ordnum_sh", DbType.Int32);
                command.Prepare();
                ParameterUtil.SetParameterValue(command, "ordnum_sh", id);
                return command.ExecuteScalar() != null;
            }
        }

        public IEnumerable<Showing> FindAll()
        {
            string query = "select ordnum_sh, date_sh, time_sh, numofspec_sh, play_id_pl, scene_id_sc from showing";
            List<Showing> showingList = new List<Showing>();

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
                            Showing showing = new Showing(reader.GetInt32(0), reader.GetDateTime(1),
                                reader.GetDateTime(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5));
                            showingList.Add(showing);
                        }
                    }
                }
            }
            return showingList;
        }

        public IEnumerable<Showing> FindAllById(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public Showing FindById(int id)
        {
            throw new NotImplementedException();
        }

        public int Save(Showing entity)
        {
            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                return Save(entity, connection);
            }
        }

        public int SaveAll(IEnumerable<Showing> entities)
        {
            int numSaved = 0;

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();
                foreach (Showing entity in entities)
                {
                    numSaved += Save(entity, connection);
                }

                transaction.Commit();

                return numSaved;
            }
        }

        public int Save(Showing entity, IDbConnection connection)
        {
            String insertSql = "" + 
                "insert into showing (date_sh, time_sh, numofspec_sh, play_id_pl, scene_id_sc, ordnum_sh) " +
                "values (:date_sh, :time_sh, :numofspec_sh, :play_id_pl, :scene_id_sc, :ordnum_sh)";
            String updateSql = "" + 
                "update showing set date_sh=:date_sh, time_sh = :time_sh, numofspec_sh = :numofspec_sh, play_id_pl = :play_id_pl, " +
                "scene_id_sc = :scene_id_sc where ordnum_sh = :ordnum_sh";

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = ExistsById(entity.OrdNumSh, connection) ? updateSql : insertSql;

                ParameterUtil.AddParameter(command, "date_sh", DbType.Date);
                ParameterUtil.AddParameter(command, "time_sh", DbType.Time);
                ParameterUtil.AddParameter(command, "numofspec_sh", DbType.Int32);
                ParameterUtil.AddParameter(command, "play_id_pl", DbType.Int32);
                ParameterUtil.AddParameter(command, "scene_id_sc", DbType.Int32);
                ParameterUtil.AddParameter(command, "ordnum_sh", DbType.Int32);

                ParameterUtil.SetParameterValue(command, "ordnum_sh", entity.OrdNumSh);
                ParameterUtil.SetParameterValue(command, "date_sh", entity.DateSh);
                ParameterUtil.SetParameterValue(command, "time_sh", entity.TimeSh);
                ParameterUtil.SetParameterValue(command, "numofspec_sh", entity.NumOfSpecSh);
                ParameterUtil.SetParameterValue(command, "play_id_pl", entity.PlayIdPl);
                ParameterUtil.SetParameterValue(command, "scene_id_sc", entity.SceneIdSc);

                return command.ExecuteNonQuery();
            }            
        }

        public List<PlayStatsDTO> FindBySceneId(int idSc)
        {
            string query = "" + 
                "select play_id_pl, sum(numofspec_sh) " +
                "from showing where scene_id_sc = :id_sc " +
                "group by play_id_pl";
            List<PlayStatsDTO> showingList = new List<PlayStatsDTO>();

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    ParameterUtil.AddParameter(command, "id_sc", DbType.Int32);
                    command.Prepare();
                    ParameterUtil.SetParameterValue(command, "id_sc", idSc);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PlayStatsDTO showingScene = new PlayStatsDTO(reader.GetInt32(0), reader.GetInt32(1));
                            showingList.Add(showingScene);
                        }
                    }
                }
            }
            return showingList;
        }

        public List<PlayShowingsStatsDTO> FindSumAvgCountForShowingPlay()
        {
            List<PlayShowingsStatsDTO> result = new List<PlayShowingsStatsDTO>();
            string query = "" + 
                "SELECT play_id_pl, SUM(numofspec_sh), AVG(numofspec_sh) , COUNT(*) " +
                "FROM Showing " +
                "GROUP BY play_id_pl";

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PlayShowingsStatsDTO showing = new PlayShowingsStatsDTO(reader.GetInt32(0), reader.GetInt32(1), reader.GetFloat(2), reader.GetInt32(3));
                            result.Add(showing);
                        }
                    }
                }
            }

            return result;
        }
        
        public List<ShowingDeleteDTO> FindShowingForDeleting(IDbConnection connection)
        {
            string query = "" + 
                "select name_sc, numofseats_sc, theatre_id_th, sc.id_sc, ordnum_sh, date_sh, time_sh, numofspec_sh, play_id_pl " +
                "from scene sc, showing sh " +
                "where sh.scene_id_sc = sc.id_sc and numofspec_sh > sc.numofseats_sc and date_sh > sysdate";
            List<ShowingDeleteDTO> result = new List<ShowingDeleteDTO>();

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ShowingDeleteDTO showing = new ShowingDeleteDTO(
                            reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), 
                            reader.GetInt32(3), reader.GetInt32(4), reader.GetDateTime(5), 
                            reader.GetDateTime(6), reader.GetInt32(7), reader.GetInt32(8));
                        result.Add(showing);
                    }
                }
            }           
            return result;
        }

        public List<ShowingDeleteDTO> DeleteAndInsertIntoShowing()
        {
            List<ShowingDeleteDTO> ret = null;
            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                //find showings that should be deleted
                ret = FindShowingForDeleting(connection);

                foreach(ShowingDeleteDTO deletedItem in ret)
                {
                    // delete old showing
                    // we can not call deleteById or delete since they create new connection
                    string query = "delete from showing where ordnum_sh=:ordnum_sh";                    
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = query;
                        ParameterUtil.AddParameter(command, "ordnum_sh", DbType.Int32);
                        command.Prepare();
                        ParameterUtil.SetParameterValue(command, "ordnum_sh", deletedItem.OrdNumSh);
                        command.ExecuteNonQuery();
                    }                   

                    // number of sold out showings = number of required tickets / number or seats
                    int n = deletedItem.NumOfSpecSh / deletedItem.NumOfSeatsSc;

                    // how many tickets is still needed after n sold out showings
                    int last = deletedItem.NumOfSpecSh % deletedItem.NumOfSeatsSc;

                    // needed to choose ordnum_sh for new showings later
                    // currentMax + 1 is first free id
                    int currentMax = FindMaxId(connection) + 1;

                    // add new showings that are sold out
                    for (int i = 0; i < n; i++)
                    {
                        // multiple new showings are created for one deleted, so calculate unique key values
                        Showing showing = CreateNewShowing(deletedItem, currentMax + i);
                        Save(showing, connection);
                    }

                    // add last showing that is not sold out
                    if (last != 0)
                    {
                        Showing showing = CreateNewShowing(deletedItem, currentMax + n);
                        showing.NumOfSpecSh = last;
                        Save(showing, connection);
                    }
                }
                transaction.Commit();
            }
            return ret;
        }

        private Showing CreateNewShowing(ShowingDeleteDTO pold, int newOrdNum)
        {
            Showing newSh = new Showing();

            newSh.PlayIdPl = pold.PlayIdPl;
            newSh.OrdNumSh = newOrdNum;
            newSh.NumOfSpecSh = pold.NumOfSeatsSc;
            newSh.SceneIdSc = pold.SceneIdSc;
            newSh.DateSh = pold.DateSh;
            newSh.TimeSh = pold.TimeSh;

            return newSh;
        }

        private int FindMaxId(IDbConnection connection)
        {
            string query = "select max(ordnum_sh) from showing";

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.Prepare();

                object result = command.ExecuteScalar();
                if (result == null) return -1;
                return Convert.ToInt32(result);
            }
        }

        public List<Showing> FindShowingByPlayId(int plId)
        {
            string query = "" + 
                "select ordnum_sh, date_sh, time_sh, numofspec_sh, play_id_pl, scene_id_sc " +
                "from showing " +
                "where play_id_pl = :play_id_pl";
            List<Showing> showingList = new List<Showing>();

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    ParameterUtil.AddParameter(command, "play_id_pl", DbType.Int32);
                    command.Prepare();
                    ParameterUtil.SetParameterValue(command, "play_id_pl", plId);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Showing showing = new Showing(
                                reader.GetInt32(0), reader.GetDateTime(1), reader.GetDateTime(2), 
                                reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5));
                            showingList.Add(showing);
                        }
                    }
                }
            }
            return showingList;
        }
    }
}
