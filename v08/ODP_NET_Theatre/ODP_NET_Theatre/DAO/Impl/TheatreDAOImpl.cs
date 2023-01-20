using ODP_NET_Theatre.ConnectionPool;
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
    public class TheatreDAOImpl : ITheatreDAO
    {
        public int Count()
        {
            string query = "select count(*) from theatre";

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

        public int Delete(Theatre entity)
        {
            return DeleteById(entity.IdTh);
        }

        public int DeleteAll()
        {
            string query = "delete from theatre";

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.Prepare();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int DeleteById(int id)
        {
            string query = "delete from theatre where id_th=:id_th";

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    ParameterUtil.AddParameter(command, "id_th", DbType.Int32);
                    command.Prepare();
                    ParameterUtil.SetParameterValue(command, "id_th", id);

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

        // connection is a parameter because this method is used in a transaction
        private bool ExistsById(int id, IDbConnection connection)
        {
            string query = "select * from theatre where id_th=:id_th";

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = query;
                ParameterUtil.AddParameter(command, "id_th", DbType.Int32);
                command.Prepare();
                ParameterUtil.SetParameterValue(command, "id_th", id);

                return command.ExecuteScalar() != null;
            }
        }

        public IEnumerable<Theatre> FindAll()
        {
            string query = "select id_th, name_th, address_th, website_th, place_id_pl from theatre";
            List<Theatre> theatreList = new List<Theatre>();

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
                            Theatre theatre = new Theatre(
                                reader.GetInt32(0), reader.GetString(1),
                                reader.GetString(2), reader.GetString(3), reader.GetInt32(4));
                            theatreList.Add(theatre);
                        }
                    }
                }
            }
            return theatreList;
        }

        public IEnumerable<Theatre> FindAllById(IEnumerable<int> ids)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("select id_th, name_th, address_th, website_th, place_id_pl from theatre where id_th in (");
            foreach (int id in ids)
            {
                sb.Append(":id" + id + ",");
            }
            sb.Remove(sb.Length - 1, 1); // delete last ','
            sb.Append(")");

            List<Theatre> theatreList = new List<Theatre>();

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = sb.ToString();
                    foreach (int id in ids)
                    {
                        ParameterUtil.AddParameter(command, "id" + id, DbType.Int32);
                    }
                    command.Prepare();
                    foreach (int id in ids)
                    {
                        ParameterUtil.SetParameterValue(command, "id" + id, id);
                    }

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Theatre theatre = new Theatre(
                                reader.GetInt32(0), reader.GetString(1),
                                reader.GetString(2), reader.GetString(3), reader.GetInt32(4));
                            theatreList.Add(theatre);
                        }
                    }
                }
            }
            return theatreList;
        }

        public Theatre FindById(int id)
        {
            string query = "" + 
                "select id_th, name_th, address_th, website_th, place_id_pl " +
                "from theatre where id_th = :id_th";
            Theatre theatre = null;

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    ParameterUtil.AddParameter(command, "id_th", DbType.Int32);
                    command.Prepare();
                    ParameterUtil.SetParameterValue(command, "id_th", id);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            theatre = new Theatre(
                                reader.GetInt32(0), reader.GetString(1),
                                reader.GetString(2), reader.GetString(3), reader.GetInt32(4));
                        }
                    }
                }
            }
            return theatre;
        }

        public int Save(Theatre entity)
        {
            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                return Save(entity, connection);
            }
        }

        // used by save and saveAll
        private int Save(Theatre theatre, IDbConnection connection)
        {
            // id_th intentionally in the last place, so that the order between commands remains the same
            string insertSql = "" + 
                "insert into theatre (name_th, address_th, website_th, place_id_pl, id_th) " +
                "values (:name_th, :address_th , :website_th, :place_id_pl, :id_th)";
            string updateSql = "" + 
                "update theatre set name_th=:name_th, address_th=:address_th, " +
                "website_th=:website_th, place_id_pl=:place_id_pl where id_th=:id_th";

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = ExistsById(theatre.IdTh, connection) ? updateSql : insertSql;

                ParameterUtil.AddParameter(command, "name_th", DbType.String, 50);
                ParameterUtil.AddParameter(command, "address_th", DbType.String, 50);
                ParameterUtil.AddParameter(command, "website_th", DbType.String, 50);
                ParameterUtil.AddParameter(command, "place_id_pl", DbType.String, 50);
                ParameterUtil.AddParameter(command, "id_th", DbType.Int32);

                command.Prepare();

                ParameterUtil.SetParameterValue(command, "id_th", theatre.IdTh);            
                ParameterUtil.SetParameterValue(command, "name_th", theatre.NameTh);        
                ParameterUtil.SetParameterValue(command, "address_th", theatre.AddressTh);
                ParameterUtil.SetParameterValue(command, "website_th", theatre.WebsiteTh);
                ParameterUtil.SetParameterValue(command, "place_id_pl", theatre.PlaceIdPl);

                return command.ExecuteNonQuery();
            }
        }

        public int SaveAll(IEnumerable<Theatre> entities)
        {            
            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                IDbTransaction transaction = connection.BeginTransaction();

                int numSaved = 0;

                foreach (Theatre entity in entities)
                {
                    numSaved += Save(entity, connection);
                }

                transaction.Commit();

                return numSaved;
            }
        }
    }
}
