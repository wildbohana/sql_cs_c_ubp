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
    public static class Example02_Updating
    {
        static readonly string query = "select spr, ruk, nap, nar from projekat";

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

                        using (OracleDataAdapter adapter = new OracleDataAdapter((OracleCommand)command))
                        {
                            DataSet dataSet = new DataSet();
                            adapter.Fill(dataSet);

                            DataTable table = dataSet.Tables[0];
                            SelectAllRows(connection);

                            InsertRow(adapter, table);
                            SelectAllRows(connection);

                            UpdateRow(adapter, table);
                            SelectAllRows(connection);

                            DeleteRow(adapter, table);
                            SelectAllRows(connection);
                        }
                    }
                }
                catch (DbException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void PrintDataTable(DataTable table)
        {
            Console.WriteLine("{0,-4} {1,-12} {2,-20} {3,-15}", "SPR", "RUKOVODILAC", "NAZIV", "NARUCILAC");
            foreach (DataRow row in table.Rows)
            {
                Console.WriteLine("{0,-4} {1,-12} {2,-20} {3,-15}", row["spr"], row["ruk"], row["nap"], row["nar"]);
            }
            Console.WriteLine();
        }

        private static void SelectAllRows(IDbConnection connection)
        {
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = "select spr, ruk, nap, nar from projekat";
                using (IDataReader reader = command.ExecuteReader())
                {
                    if (((DbDataReader)reader).HasRows)
                    {
                        Console.WriteLine("{0,-4} {1,-12} {2,-20} {3,-15}", "SPR", "RUKOVODILAC", "NAZIV", "NARUCILAC");

                        while (reader.Read())
                        {
                            Console.WriteLine("{0,-4} {1,-12} {2,-20} {3,-15}",
                                reader.GetInt32(0), 
                                reader.GetInt32(1), 
                                reader.GetString(2), 
                                reader.GetString(3));
                        }
                    }
                }
            }
            Console.WriteLine();
        }

        private static void InsertRow(OracleDataAdapter adapter, DataTable table)
        {
            DataRow row = table.NewRow();
            row["spr"] = 101;
            row["ruk"] = 50;
            row["nap"] = "ODP.NET Projekat";
            row["nar"] = "BP1PSI";

            table.Rows.Add(row);
            
            using (DbCommandBuilder commandBuilder = new OracleCommandBuilder(adapter))
            {
                adapter.Update(table); // (moze se proslediti i DataSet)
            }
        }

        private static void UpdateRow(OracleDataAdapter adapter, DataTable table)
        {
            table.Rows[table.Rows.Count-1]["nap"] = "Izmena naziva";

            using (DbCommandBuilder commandBuilder = new OracleCommandBuilder(adapter))
            {
                adapter.Update(table);
            }
        }

        private static void DeleteRow(OracleDataAdapter adapter, DataTable table)
        {
            table.Rows[table.Rows.Count - 1].Delete();

            using (DbCommandBuilder commandBuilder = new OracleCommandBuilder(adapter))
            {
                adapter.Update(table);
            }
        }
    }
}
