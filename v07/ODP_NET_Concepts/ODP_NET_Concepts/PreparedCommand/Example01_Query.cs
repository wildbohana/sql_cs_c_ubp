using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.ManagedDataAccess.Client;

namespace ODP_NET_Concepts.PreparedCommand
{
	public static class Example01_Query
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
						command.CommandText = GenerateQuery();
						command.Prepare();

						using (IDataReader reader = command.ExecuteReader())
						{

							Console.WriteLine("Radnici: ");
							Console.WriteLine("{0,-4} {1,-8} {2,-8} {3,-2}", "MBR", "IME", "PREZIME", "BROJ_PROJEKATA");

							while (reader.Read())
							{
								int mbr = reader.GetInt32(0);
								string ime = reader.GetString(1);
								string prezime = reader.GetString(2);
								int broj_projekata = reader.GetInt32(3);

								Console.WriteLine("{0,-4} {1,-8} {2,-8} {3,-2}", mbr, ime, prezime, broj_projekata);
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

		private static string GenerateQuery()
		{
			string query = "" +
				"select radnik.mbr, ime, prz prezime, count(spr) br_projekata " +
				"from radnik left outer join radproj " +
				"on radnik.mbr = radproj.mbr group by radnik.mbr, ime, prz " +
				"having count(spr)<3 order by br_projekata desc, mbr asc";

			return query;
		}
	}
}
