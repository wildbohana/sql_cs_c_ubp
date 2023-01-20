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
    public class RoleDAOImpl : IRoleDAO
    {
        public int Count()
        {
            throw new NotImplementedException();
        }

        public int Delete(Role entity)
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

        public IEnumerable<Role> FindAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Role> FindAllById(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public Role FindById(int id)
        {
            throw new NotImplementedException();
        }
        public int FindCountByPlayId(int idPl)
        {
            string query = "" +
                "select count(*) from role " +
                "where play_id_pl=:id_pl";

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    ParameterUtil.AddParameter(command, "id_pl", DbType.Int32);
                    command.Prepare();

                    ParameterUtil.SetParameterValue(command, "id_pl", idPl);
                    object result = command.ExecuteScalar();
                    int resultAsInt = Convert.ToInt32(result);

                    return resultAsInt;
                }
            }
        }

        public int FindCountForRoleGender(int idPl, string gender)
        {
            string query = "" +
                "select count(gender_ro) from role " +
                "where play_id_pl=:play_id_pl and gender_ro=:gender_ro";

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    ParameterUtil.AddParameter(command, "play_id_pl", DbType.Int32);
                    ParameterUtil.AddParameter(command, "gender_ro", DbType.StringFixedLength, 1);
                    command.Prepare();

                    ParameterUtil.SetParameterValue(command, "play_id_pl", idPl);
                    ParameterUtil.SetParameterValue(command, "gender_ro", gender);

                    object result = command.ExecuteScalar();

                    if (result == null) return -1;

                    return Convert.ToInt32(result);
                }
            }
        }

        public List<Role> FindRoleByPlayId(int idPl)
        {
            string query = "" +
                "select id_ro, name_ro, gender_ro, type_ro, play_id_pl " +
                "from role " +
                "where play_id_pl = :play_id_pl";

            List<Role> result = new List<Role>();

            using (IDbConnection connection = ConnectionUtil_Pooling.GetConnection())
            {
                connection.Open();
                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    ParameterUtil.AddParameter(command, "play_id_pl", DbType.Int32);                    
                    command.Prepare();

                    ParameterUtil.SetParameterValue(command, "play_id_pl", idPl);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Role u = new Role(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4));
                            result.Add(u);
                        }
                    }
                }
            }
            return result;
        }

        public int Save(Role entity)
        {
            throw new NotImplementedException();
        }

        public int SaveAll(IEnumerable<Role> entities)
        {
            throw new NotImplementedException();
        }
    }
}
