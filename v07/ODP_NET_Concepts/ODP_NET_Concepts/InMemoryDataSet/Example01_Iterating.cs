using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;

namespace ODP_NET_Concepts.InMemoryDataSet
{
    public static class Example01_Iterating
    {
        static readonly string query = "select mbr, ime, prz, plt from radnik";

        public static void Example()
        {
            using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = query;

                        using (DbDataAdapter adapter = new OracleDataAdapter((OracleCommand)command)) // gubimo DB-nezavisnost
                        {
                            DataSet dataSet = new DataSet();
                            adapter.Fill(dataSet);

                            DataTable dataTable = dataSet.Tables[0];

                            Console.WriteLine("{0,-4} {1,-8} {2,-10} {3,8:F2}", "MBR", "IME", "PREZIME", "PLATA");
                            foreach (DataRow row in dataTable.Rows)
                            {
                                Console.WriteLine("{0,-4} {1,-8} {2,-10} {3,8:F2}", row["mbr"], row["ime"], row["prz"], row["plt"]);
                            }
                            Console.WriteLine();
                        }
                    }
                }
                catch (DbException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
