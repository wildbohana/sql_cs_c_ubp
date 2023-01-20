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
    public static class Example02_QueryWithParams
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
						command.CommandText = "select mbr, ime, prz, plt from radnik where ime like :ime and prz like :prz"; 

						ParameterUtil.AddParameter(command, "ime",		DbType.String, 25);	
						ParameterUtil.AddParameter(command, "prezime",	DbType.String, 25);	
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
				}
				catch (DbException ex)
				{
					Console.WriteLine(ex.Message);
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

