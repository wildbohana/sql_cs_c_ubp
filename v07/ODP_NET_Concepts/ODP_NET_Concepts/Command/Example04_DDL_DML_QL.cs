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
    public class Example04_DDL_DML_QL
    {
        public static void Example()
        {
            //createTableFazeProjekta();

            HandleMenu();
        }

        private static void HandleMenu()
        {
			String response = null;
			do
			{
				Console.WriteLine("Choose an option:");
				Console.WriteLine("0 - Create table Faze_Projekta");
				Console.WriteLine("1 - Select all rows from Faze_Projekta");
				Console.WriteLine("2 - Select row from Faze_Projekta by spr and sfp");
				Console.WriteLine("3 - Insert row into Faze_Projekta");
				Console.WriteLine("4 - Modify existing row from Faze_Projekta");
				Console.WriteLine("5 - Delete existing row from Faze_Projekta");
				Console.WriteLine("6 - Drop table Faze_Projekta");
				Console.WriteLine("X - Exit");

				response = Console.ReadLine();

				switch (response)
				{
					case "0":
						CreateTableFazeProjekta();
						break;
					case "1":
						SelectAll();
						break;
					case "2":
						SelectBySprAndSfp();
						break;
					case "3":
						InsertRow();
						break;
					case "4":
						ModifyRow();
						break;
					case "5":
						DeleteRow();
						break;
					case "6":
						DropTableFazeProjekta();
						break;
				}

				Console.WriteLine();

			} while (!response.ToUpper().Equals("X"));
		}

		// Izvrsava DML i DDL naredbe
		private static int ExecuteNonQuery(string sql)
		{
			using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection())
			{
				try
				{
					connection.Open();
					using (IDbCommand command = connection.CreateCommand())
					{
						command.CommandText = sql;
						int rowsAffected = command.ExecuteNonQuery();
						return rowsAffected;
					}
				}
				catch (DbException ex)
				{
					Console.WriteLine(ex.Message);
					return -2;
				}
			}
		}

		private static void CreateTableFazeProjekta()
		{
			string sql = "" +
				"create table faze_projekta " + 
				"(spr int, sfp int, rukfp int, nafp varchar2(10), datp date, " +
				"constraint fp_pk primary key (spr, sfp), " +
				"constraint fp_fk foreign key (rukfp) " +
				"references radnik(mbr))";

			int rowsAffected = ExecuteNonQuery(sql);
			if (rowsAffected != -2)
			{
				Console.WriteLine("Table Faze_Projekta successfully created!");
			}
			else
			{
				Console.WriteLine("Exception occurred!");
			}
		}

		private static void SelectAll()
		{
			string sql = "select spr, sfp, rukfp, nafp, datp from faze_projekta";

			using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection())
			{
				try
				{
					connection.Open();
					using (IDbCommand command = connection.CreateCommand())
					{
						command.CommandText = sql;
						using (IDataReader reader = command.ExecuteReader())
						{
							Console.WriteLine("{0,-4} {1,-4} {2,-6} {3,-20} {4,-21}", "SPR", "SFP", "RUKFP", "NAFP", "DATP");

							while (reader.Read())
							{
								Console.WriteLine("{0,-4} {1,-4} {2,-6} {3,-20} {4,-21}",
									reader.IsDBNull(0) ? "" : reader.GetString(0),
									reader.IsDBNull(1) ? "" : reader.GetString(1),
									reader.IsDBNull(2) ? "" : reader.GetString(2),
									reader.IsDBNull(3) ? "" : reader.GetString(3),
									reader.IsDBNull(4) ? "" : reader.GetString(4));
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

		private static void SelectBySprAndSfp()
		{
			Console.WriteLine("SPR: ");
			string spr = Console.ReadLine();
			Console.WriteLine("SFP: ");
			string sfp = Console.ReadLine();

			string sql = string.Format("select * from faze_projekta where spr = {0} and sfp = {1}", spr, sfp);

			using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection())
			{
				try
				{
					connection.Open();
					using (IDbCommand command = connection.CreateCommand())
					{
						command.CommandText = sql;
						using (IDataReader reader = command.ExecuteReader())
						{
							Console.WriteLine("{0,-4} {1,-4} {2,-6} {3,-20} {4,-21}", "SPR", "SFP", "RUKFP", "NAFP", "DATP");

							if (((DbDataReader)reader).HasRows)
							{
								while (reader.Read())
								{
									Console.WriteLine("{0,-4} {1,-4} {2,-6} {3,-20} {4,-21}",
										reader.IsDBNull(0) ? "" : reader.GetString(0),
										reader.IsDBNull(1) ? "" : reader.GetString(1),
										reader.IsDBNull(2) ? "" : reader.GetString(2),
										reader.IsDBNull(3) ? "" : reader.GetString(3),
										reader.IsDBNull(4) ? "" : reader.GetString(4));
								}
							}
							else
							{
								Console.WriteLine("No rows selected!");
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

		private static void InsertRow()
		{
			Console.WriteLine("SPR: ");
			string spr = Console.ReadLine();
			Console.WriteLine("SFP: ");
			string sfp = Console.ReadLine();
			Console.WriteLine("RUKFP: ");
			string rukfp = Console.ReadLine();
			Console.WriteLine("NAFP: ");
			string nafp = Console.ReadLine();
			Console.WriteLine("DATP (DD-MM-YYYY): ");
			string datp = Console.ReadLine();

			// u Oracle SQLu, to_date(null, 'dd.mm.yyyy') vraća null
			// null mora biti naveden bez navodnika pa pri unosu datuma to uzimamo u obzir

			string sql = string.Format(
				"insert into faze_projekta (spr, sfp, rukfp, nafp, datp) " +
				"values ({0}, {1}, {2}, '{3}', to_date({4}, 'dd.MM.yyyy.'))",
				string.IsNullOrEmpty(spr)   ? "null" : spr,
				string.IsNullOrEmpty(sfp)   ? "null" : sfp,
				string.IsNullOrEmpty(rukfp) ? "null" : rukfp,
				string.IsNullOrEmpty(nafp)  ? "null" : nafp,
				string.IsNullOrEmpty(datp)  ? "null" : string.Format("'{0}'", datp)
			);

			int rowsAffected = ExecuteNonQuery(sql);
			if (rowsAffected != -2)
			{
				Console.WriteLine("Row sucessfully inserted");
			}
			else
			{
				Console.WriteLine("Exception occurred!");
			}
		
		}

		private static void ModifyRow()
		{
			Console.WriteLine("SPR: ");
			string spr = Console.ReadLine();
			Console.WriteLine("SFP: ");
			string sfp = Console.ReadLine();
			Console.WriteLine("RUKFP: ");
			string rukfp = Console.ReadLine();
			Console.WriteLine("NAFP: ");
			string nafp = Console.ReadLine();
			Console.WriteLine("DATP (DD-MM-YYYY): ");
			string datp = Console.ReadLine();

			string sql = string.Format(
					"update faze_projekta set rukfp={0}, nafp='{1}', datp=to_date({2}, 'dd.MM.yyyy.') where spr={3} and sfp={4}",
					string.IsNullOrEmpty(rukfp) ? "null" : rukfp,
					string.IsNullOrEmpty(nafp) ? "null" : nafp,
					string.IsNullOrEmpty(datp) ? "null" : string.Format("'{0}'", datp),
					string.IsNullOrEmpty(spr) ? "null" : spr,
					string.IsNullOrEmpty(sfp) ? "null" : sfp);

			int rowsAffected = ExecuteNonQuery(sql);
			if (rowsAffected != -2)
			{
				Console.WriteLine("{0} row(s) affected by update!", rowsAffected);
			}
			else
			{
				Console.WriteLine("Exception occurred!");
			}

		}

		private static void DeleteRow()
		{
			Console.WriteLine("SPR: ");
			string spr = Console.ReadLine();
			Console.WriteLine("SFP: ");
			string sfp = Console.ReadLine();

			string sql = string.Format(
				"delete from faze_projekta where spr={0} and sfp={1}",
				string.IsNullOrEmpty(spr) ? "null" : spr,
				string.IsNullOrEmpty(sfp) ? "null" : sfp);

			int rowsAffected = ExecuteNonQuery(sql);
			if (rowsAffected != -2)
			{
				Console.WriteLine("{0} row(s) affected by delete!", rowsAffected);
			}
			else
			{
				Console.WriteLine("Exception occurred!");
			}
		}

		private static void DropTableFazeProjekta()
		{
			string sql = "drop table faze_projekta";

			int rowsAffected = ExecuteNonQuery(sql);
			if (rowsAffected != -2)
			{
				Console.WriteLine("Table Faze_Projekta successfully dropped!");
			}
			else
			{
				Console.WriteLine("Exception occurred!");
			}
		}
	}
}
