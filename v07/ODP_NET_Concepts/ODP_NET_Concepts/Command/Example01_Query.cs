using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;

namespace ODP_NET_Concepts.Command
{
    public static class Example01_Query
    {
        public static void Example ()
        {
            IDbConnection connection = null;
            IDbCommand command = null;
            IDataReader reader = null;

            OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
            ocsb.DataSource = Connection.ConnectionParams.LOCAL_DATA_SOURCE;
            ocsb.UserID = Connection.ConnectionParams.USER_ID;
            ocsb.Password = Connection.ConnectionParams.PASSWORD;
            connection = new OracleConnection(ocsb.ConnectionString);

            try
            {
                connection.Open();

                command = connection.CreateCommand();
                command.CommandText = "" +
                    "select radnik.mbr, ime, prz prezime, plt, count(spr) br_projekata " +
                    "from radnik left outer join radproj on radnik.mbr = radproj.mbr " +
                    "group by radnik.mbr, ime, prz, plt " +
                    "having count(spr)<3 " +
                    "order by br_projekata desc, mbr asc";
                reader = command.ExecuteReader();

                Console.WriteLine("{0,-4} {1,-8} {2,-8} {3,8:F2} {4,14}", "MBR", "IME", "PREZIME", "PLATA", "BROJ_PROJEKATA");

                while (reader.Read())
                {
                    int mbr = reader.GetInt32(0);
                    string ime = reader.GetString(1);
                    string prz = reader.GetString(2);
                    float plt = reader.GetFloat(3);
                    int brojProjekata = reader.GetInt32(4);

                    Console.WriteLine("{0,-4} {1,-8} {2,-8} {3,8:F2} {4,14}", mbr, ime, prz, plt, brojProjekata);
                }
            }
            catch (DbException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

                if (command != null)
                {
                    command.Dispose();
                }

                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }

                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
        }
    }
}
