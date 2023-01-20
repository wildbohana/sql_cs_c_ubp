using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODP_NET_Concepts.Util;
using Oracle.ManagedDataAccess.Client;

namespace ODP_NET_Concepts.ConnectionPool
{
	class Example01_ConnectionPool
	{
		public static void Example()
		{
			using (IDbConnection connection = ConnectionPool.ConnectionUtil_Pooling.GetConnection())
			{
				using (IDbCommand command = connection.CreateCommand())
				{
					try
					{
						connection.Open();

						command.CommandText = "select mbr, ime, prz, plt from radnik where ime like :ime and prz like :prz"; 

						command.Parameters.Add(new OracleParameter("ime", OracleDbType.Varchar2, 25));
						command.Parameters.Add(new OracleParameter("prezime", OracleDbType.Varchar2, 25));
						command.Prepare();

						string answer;
						do
						{
							Console.WriteLine("Odaberite opciju:");
							Console.WriteLine("1 - Odabir radnika po imenu i prezimenu");
							Console.WriteLine("X - Izlazak iz programa");

							answer = Console.ReadLine();

							if (answer.Equals("1"))
							{
								SelectRadnikByImeAndPrz(command);
							}

							Console.WriteLine();
						} while (!answer.ToUpper().Equals("X"));
					}
					catch (OracleException ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
		}

		private static void SelectRadnikByImeAndPrz(IDbCommand preparedCommand)
		{
			Console.WriteLine("Ime: ");
			ParameterUtil.SetParameterValue(preparedCommand, "ime", Console.ReadLine());

			Console.WriteLine("Prezime: ");
			ParameterUtil.SetParameterValue(preparedCommand, "prezime", Console.ReadLine());

			using (IDataReader reader = preparedCommand.ExecuteReader())
			{
				if (((DbDataReader)reader).HasRows)
				{
					Console.WriteLine("{0,-4} {1,-10} {2,-10} {3,8:F2}", "MBR", "IME", "PREZIME", "PLATA");
					while (reader.Read())
					{
						int mbr = reader.GetInt32(0);
						String ime = reader.GetString(1);
						String prezime = reader.GetString(2);
						float plata = reader.GetFloat(3);
						Console.WriteLine("{0,-4} {1,-10} {2,-10} {3,8:F2}", mbr, ime, prezime, plata);
					}
				}
				else
				{
					Console.WriteLine("No data found!");
				}
			}
		}
	}
}
