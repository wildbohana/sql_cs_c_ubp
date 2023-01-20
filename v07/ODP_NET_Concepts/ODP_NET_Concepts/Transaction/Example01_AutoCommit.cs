using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;

namespace ODP_NET_Concepts.Transaction
{
	public static class Example01_AutoCommit
	{
		public static void Example()
		{
			CreateTempTables();

			Console.WriteLine("--- DATA BEFORE UPDATE ---");
			PrintJoinRadnik2RadProj2();

			ExecuteSimple("update radproj2 set brc = brc * 0.5");
			ExecuteSimple("update radnik2 set plt = plt * 0.5");

			Console.WriteLine("\n--- DATA AFTER UPDATE ---");
			PrintJoinRadnik2RadProj2();

			DropTempTables();
		}

		private static void PrintJoinRadnik2RadProj2() 
		{
			string query = "" +
				"select r.mbr, ime, prz, plt, spr, brc " +
				"from radnik2 r inner join radproj2 rp on r.mbr = rp.mbr";

			using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection())
			{
				try 
				{
					connection.Open();
					using (IDbCommand command = connection.CreateCommand())
					{
						command.CommandText = query;
						using (IDataReader reader = command.ExecuteReader())
						{
							Console.WriteLine("Radnici:");
							Console.WriteLine("{0,-4} {1,-8} {2,-9} {3,8:F2} {4,3} {5,3}", "MBR", "IME", "PREZIME", "PLATA", "SPR", "BRC");

							while (reader.Read())
							{
								int mbr = reader.GetInt32(0);
								string ime = reader.GetString(1);
								string prz = reader.GetString(2);
								float plt = reader.GetFloat(3);
								int brojProjekata = reader.GetInt32(4);

								Console.WriteLine("{0,-4} {1,-8} {2,-9} {3,8:F2} {4,3} {5,3}",
									reader.GetInt32(0), reader.GetString(1), 
									reader.GetString(2), reader.GetFloat(3),
									reader.GetInt32(4), reader.GetInt32(5));
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

		private static void CreateTempTables() 
		{
			string createTableRadnik2Command = "" +
				"create table radnik2 as " +
				"select mbr, ime, prz, plt from radnik" +
				" where mbr in(select mbr from radproj where spr=10)";
			string alterTableRadnik2Command = "" +
				"alter table radnik2 " +
				"add constraint CH_PLT check (plt > 5000)";
			string createTableRadProj2Command = "" +
				"create table radproj2 " + 
				"as select mbr, spr, brc " +
				"from radproj where spr=10";

			ExecuteSimple(createTableRadnik2Command);
			ExecuteSimple(alterTableRadnik2Command);
			ExecuteSimple(createTableRadProj2Command);
		}

		private static void DropTempTables() 
		{
			string dropTableRadnik2Command = "drop table radnik2";
			string dropTableRadProj2Command = "drop table radproj2";

			ExecuteSimple(dropTableRadnik2Command);
			ExecuteSimple(dropTableRadProj2Command);
		}

		private static bool ExecuteSimple(String sql)
		{
			using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection()) 
			{
				try 
				{
					connection.Open();
					using (IDbCommand command = connection.CreateCommand())
					{
						command.CommandText = sql;
						try
						{
							command.ExecuteNonQuery();
							Console.WriteLine("DML/DDL command executed successfully!");
							return true;
						}
						catch (DbException ex)
						{
							Console.WriteLine();
							Console.WriteLine("DML/DDL command failed with the following exception:");
							Console.WriteLine(ex.Message);
							return false;
						}
					}
				}
				catch (DbException ex)
				{
					Console.WriteLine(ex.Message);
					return false;
				}
			}
		}
	}
}
