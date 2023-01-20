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
    public static class Example03_QueryWithParams
    {
        public static void Example()
        {
            using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = generateQuery();
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            if (((DbDataReader)reader).HasRows)
                            {
                                Console.WriteLine("{0,-4} {1}", "SPR", "NAZIV PROJEKTA");

                                while (reader.Read())
                                {
                                    int spr = reader.GetInt32(0);
                                    string nap = reader.GetString(1);

                                    Console.WriteLine("{0,-4} {1}", spr, nap);
                                }
                            }
                        }
                    }
                }
                catch (DbException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static string generateQuery()
        {
            Console.WriteLine("MBR: ");
            String mbr = Console.ReadLine();

            String query = "" +
                "select spr, nap from projekat where spr in" +
                "(select spr from radproj where mbr = " + mbr + ")";

            return query;
        }
    }
}
