// String sa zahtevom za bazu
string query = "select spr, ruk, nap, nar from projekat";

// Ne znam iskreno
void Azuriranje()
{
	using (IDbConnection connection = Connection.ConnectionUtil.GetConnection())
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

void PrintDataTable(DataTable table)
{
	Console.WriteLine("SPR: /t/tRUK: /t/tNAZ: /t/tNAR:");

	foreach (DataRow row in table.Rows)
	{
		Console.WriteLine(row["spr"], row["ruk"], row["nap"], row["nar"]);
	}

	Console.WriteLine();
}

void SelectAllRows(IDbConnection connection)
{
	using (IDbCommand command = connection.CreateCommand())
	{
		command.CommandText = query;

		using (IDataReader reader = command.ExecuteReader())
		{
			if (((DbDataReader)reader).HasRows)
			{
				Console.WriteLine("SPR: /t/tRUK: /t/tNAZ: /t/tNAR:");

				while (reader.Read())
				{
					// Da li su dobri tipovi
					int spr = reader.GetInt32(0);
					string ruk = reader.GetString(1);
					string nap = reader.GetString(2);
					int nar = reader.GetInt32(3);

					Console.WriteLine(spr, ruk, nap, nar);
				}
			}
		}
	}
	Console.WriteLine();
}

void InsertRow(OracleDataAdapter adapter, DataTable table)
{
	DataRow row = table.NewRow();
	row["spr"] = 101;
	row["ruk"] = 50;
	row["nap"] = "ODP.NET Projekat";
	row["nar"] = "UBP PSI";

	table.Rows.Add(row);

	using (DbCommandBuilder commandBuilder = new OracleCommandBuilder(adapter))
	{
		adapter.Update(table);
	}
}

void UpdateRow(OracleDataAdapter adapter, DataTable table)
{
	table.Rows[table.Rows.Count-1]["nap"] = "Izmena naziva";

	using (DbCommandBuilder commandBuilder = new DbCommandBuilder(adapter))
	{
		adapter.Update(table);
	}
}

void DeleteRow(OracleDataAdapter adapter, DataTable table)
{
	table.Rows[table.Rows.Count - 1].Delete();

	using (DbCommandBuilder commandBuilder = new OracleCommandBuilder(adapter))
	{
		adapter.Update(table);
	}
}
