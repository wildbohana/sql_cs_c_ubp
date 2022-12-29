// Query
void Upit()
{
	using (IDbConnection connection = connection.ConnectionUtil.GetConnection())
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
					Console.WriteLine("MBR: /t/tIME: /t/tPRZ: /t/tBRP: ");

					while (reader.Read())
					{
						int mbr = reader.GetInt32(0);
						string ime = reader.GetString(1);
						string prezime = reader.GetString(2);
						int broj_projekata = reader.GetInt32(3);

						Console.WriteLine(mbr, ime, prezime, broj_projekata);
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

string GenerateQuery()
{
	string query = ""
		+ "select radnik.mbr, ime, prz prezime, count(spr) br_projekata "
		+ "from radnik left outer join radproj on radnik.mbr = radproj.mbr "
		+ "group by radnik.mbr, ime, prz having count(spr) < 3 "
		+ "order by br_projekata desc, mbr asc";
	
	return query;
}