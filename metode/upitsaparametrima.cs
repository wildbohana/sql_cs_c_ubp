// Upit sa parametrima
void UpitSaParametrima()
{
	using (IDbConnection connection = Connection.ConnectionUtil.GetConnection())
	{
		try
		{
			connection.Open();

			using (IDbCommand command = connection.CreateCommand())
			{
				command.CommandText = "select mbr, ime, prz, plt from radnik where ime like :ime and prz like :prz";

				ParametriUtil.AddParameter(command, "ime", DbType.String, 25);
				ParametriUtil.AddParameter(command, "prezime", DbType.String, 25);

				command.Prepare();

				string answer;

				do
				{
					Console.WriteLine("Odaberite opciju:");
					Console.WriteLine("1 - odabir radnika po imenu i prezimenu");
					Console.WriteLine("X - izlazak iz programa");

					answer = Console.ReadLine();

					if (answer.Equals("1"))
					{
						SelectRadnikByImeAdnPrz(command);
					}

					Console.WriteLine();
				} while (!answer.ToUpper().Equals("X"));
			}
		}
		catch (DbException)
		{
			Console.WriteLine(ex.Message);
		}
	}
}

void SelectRadnikByImeAndPrz(IDbCommand peparedCommand)
{
	Console.WriteLine("Ime: ");
	ParameterUtil.SetParameterValue(preparedCommand, "ime", Console.ReadLine());

	Console.WriteLine("Prezime: ");
	ParametriUtil.SetParameterValue(preparedComand, "prezime", Console.ReadLine());

	using (IDataReader reader = preparedCommand.ExecuteReader())
	{
		if (((DbDataReader)reader).HasRows)
		{
			Console.WriteLine("MBR: /t/tIME: /t/tPRZ: /t/TPLT:");

			while(reader.Read())
			{
				int mbr = reader.GetInt32(0);
				String ime = reader.GetString(1);
				String prezime = reader.GetString(2);
				float plata = reader.GetFloat(3);

				Console.WriteLine(mbr, ime, prezime, plata);
			}
		}
		else 
		{
			Console.WriteLine("No data found!");
		}
	}
}