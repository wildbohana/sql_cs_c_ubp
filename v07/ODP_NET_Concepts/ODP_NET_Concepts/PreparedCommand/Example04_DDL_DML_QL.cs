using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODP_NET_Concepts.Util;
using Oracle.ManagedDataAccess.Client;

namespace ODP_NET_Concepts.PreparedCommand
{
    public static class Example04_DDL_DML_QL
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

		private static void CreateTableFazeProjekta()
		{
			string sql = "" +
				"create table faze_projekta (spr int, sfp int, rukfp int, nafp varchar2(10), datp date, " +
				"constraint fp_pk primary key (spr, sfp), " +
				"constraint fp_fk foreign key (rukfp) " +
				"references radnik(mbr))";

			using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection())
			{
				try
				{
					connection.Open();
					using (IDbCommand command = connection.CreateCommand())
					{
						command.CommandText = sql;
						command.ExecuteNonQuery();
						Console.WriteLine("Table Faze_Projekta successfully created!");
					}
				}
				catch (DbException ex)
				{
					Console.WriteLine(ex.Message);
				}
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

			using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection())
			{
				try
				{
					connection.Open();
					using (IDbCommand command = connection.CreateCommand())
					{
						command.CommandText = "select * from faze_projekta where spr = :spr and sfp = :sfp";
						ParameterUtil.AddParameter(command, "spr", DbType.Int32, 0);
						ParameterUtil.AddParameter(command, "sfp", DbType.Int32, 0);
						command.Prepare();

						ParameterUtil.SetParameterValue(command, "spr", spr);
						ParameterUtil.SetParameterValue(command, "sfp", sfp);

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

			string sql = "" +
				"insert into faze_projekta (spr, sfp, rukfp, nafp, datp) " +
				"values (:spr, :sfp, :rukfp, :nafp, to_date(:datp, 'dd.MM.yyyy.'))";

			using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection())
			{
				try
				{
					connection.Open();
					using (IDbCommand command = connection.CreateCommand())
					{
						command.CommandText = sql;
						ParameterUtil.AddParameter(command, "spr", DbType.Int32, 0);
						ParameterUtil.AddParameter(command, "sfp", DbType.Int32, 0);
						ParameterUtil.AddParameter(command, "rukfp", DbType.Int32, 0);
						ParameterUtil.AddParameter(command, "nafp", DbType.String, 20);
						ParameterUtil.AddParameter(command, "datp", DbType.String, 10);
						command.Prepare();

						ParameterUtil.SetParameterValue(command, "spr", string.IsNullOrEmpty(spr) ? null : spr);
						ParameterUtil.SetParameterValue(command, "sfp", string.IsNullOrEmpty(sfp) ? null : sfp);
						ParameterUtil.SetParameterValue(command, "rukfp", string.IsNullOrEmpty(rukfp) ? null : rukfp);
						ParameterUtil.SetParameterValue(command, "nafp", string.IsNullOrEmpty(nafp) ? null : nafp);
						ParameterUtil.SetParameterValue(command, "datp", string.IsNullOrEmpty(datp) ? null : datp);

						int rowsAffected = command.ExecuteNonQuery();
						if (rowsAffected > 0)
							Console.WriteLine("Row sucessfully inserted");
						else
							Console.WriteLine("Insert failed!");
					}
				}
				catch (DbException ex)
				{
					Console.WriteLine(ex.Message);
				}
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

			string sql = "" +
				"update faze_projekta set " +
				"rukfp=:rukfp, nafp=:nafp, datp=to_date(:datp, 'dd.MM.yyyy.') " + 
				"where spr=:spr and sfp=:sfp";

			using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection())
			{
				try {
					connection.Open();
					using (IDbCommand command = connection.CreateCommand())
					{
						command.CommandText = sql;
						ParameterUtil.AddParameter(command, "rukfp", DbType.Int32, 0);
						ParameterUtil.AddParameter(command, "nafp", DbType.String, 20);
						ParameterUtil.AddParameter(command, "datp", DbType.String, 10);
						ParameterUtil.AddParameter(command, "spr", DbType.Int32, 0);
						ParameterUtil.AddParameter(command, "sfp", DbType.Int32, 0);
                        command.Prepare();

						ParameterUtil.SetParameterValue(command, "spr", string.IsNullOrEmpty(spr) ? null : spr);
						ParameterUtil.SetParameterValue(command, "sfp", string.IsNullOrEmpty(sfp) ? null : sfp);
						ParameterUtil.SetParameterValue(command, "rukfp", string.IsNullOrEmpty(rukfp) ? null : rukfp);
						ParameterUtil.SetParameterValue(command, "nafp", string.IsNullOrEmpty(nafp) ? null : nafp);
						ParameterUtil.SetParameterValue(command, "datp", string.IsNullOrEmpty(datp) ? null : datp);

						Console.WriteLine("{0} row(s) affected by update!", command.ExecuteNonQuery());
					}
				}
				catch (DbException ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		private static void DeleteRow()
		{
			Console.WriteLine("SPR: ");
			string spr = Console.ReadLine();
			Console.WriteLine("SFP: ");
			string sfp = Console.ReadLine();

			string sql = "delete from faze_projekta where spr=:spr and sfp=:sfp";

			using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection())
			{
				try
				{
					connection.Open();
					using (IDbCommand command = connection.CreateCommand())
					{
						command.CommandText = sql;
						ParameterUtil.AddParameter(command, "spr", DbType.Int32, 0);
						ParameterUtil.AddParameter(command, "sfp", DbType.Int32, 0);
						command.Prepare();

						ParameterUtil.SetParameterValue(command, "spr", spr);
						ParameterUtil.SetParameterValue(command, "sfp", sfp);

						Console.WriteLine("{0} row(s) affected by delete!", command.ExecuteNonQuery());
					}
				}
				catch (DbException ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		private static void DropTableFazeProjekta()
		{
			string sql = "drop table faze_projekta";

			using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection())
			{
				try
				{
					connection.Open();
					using (IDbCommand command = connection.CreateCommand())
					{
						command.CommandText = sql;
						command.ExecuteNonQuery();
						Console.WriteLine("Table Faze_Projekta successfully dropped!");
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
