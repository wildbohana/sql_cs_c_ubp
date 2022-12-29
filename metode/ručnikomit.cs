void Poziv()
{
	CreateTempTables();

	Console.WriteLine("--- DATA BEFORE UPDATE ---");
	PrintJoinRadnik2RadProj2();

	// ExecuteSimple
	ExecuteTransactional("update radproj2 set brc = brc * 0.5 update radnik2 set plt = plt * 0.5");

	Console.WriteLine("\n--- DATA AFTER UPDATE ---");
	PrintJoinRadnik2RadProj2();

	DropTempTables();
}

// Samo je ovo dodato iznad svega
void ExecuteTransactional(params string[] sqls)
{
	using (IDbConnection connection = Connection.ConnectionUtil.GetConnection())
	{
		try 
		{
			connection.Open();

			using (IDbCommand command = connection.CreateCommand())
			{
				IDbTransaction transaction = connection.BeginTransaction();
				
				try 
				{
					foreach (string sql in sqls)
					{
						command.CommandText = sql;
						command.ExecuteNonQuery();
					}
					transaction.Commit();
				}
				catch (DbException ex)
				{
					Console.WriteLine();
					Console.WriteLine("DML command failed with the following exception:");
					Console.WriteLine(ex.Message);

					Console.WriteLine("Rolling back...");
					transaction.Rollback();
				}
			}
		}
		catch (DbException ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}

void PrintJoinRadnik2RadProj2()
{
	string query = ""
		+ "select r.mbr, ime, prz, plt, spr, brc "
		+ "from radnik2 r join radproj2 rp on r.mbr = rp.mbr";

	using (IDbConnection connection = Connection.ConnectionUtil.GetConnection())
	{
		try
		{
			connection.Open();

			using (IDbCommand command = connection.CreateCommand())
			{
				command.CommandText = query;

				using (IDataReader reader = command.ExecuteReader())
				{
					Console.WriteLine("Radnici: ");
					Console.WriteLine("MBR: /t/tIME: /t/tPRZ: /t/tPLT: /t/t SPR: /t/tBRC:");

					while (reader.Read())
					{
						int mbr = reader.GetInt32(0);
						string ime = reader.GetString(1);
						string prz = reader.GetString(2);
						float plt = reader.GetFloat(3);
						int brojProjekata = reader.GetInt32(4);
						int brojCasova = reader.GetInt32(5);

						Console.WriteLine(mbr, ime, prz, plt, brojProjekata, brojCasova);
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

void CreateTempTables()
{
	string createTableRadnik2Command = ""
		+ " create table radnik2 as select mbr, ime, prz, plt "
		+ "from radnik + where mbr in(select mbr from radproj where spr=10)";
	string alterTableRadnik2Command = ""
		+ "alter table radnik2 add constraint CH_PLT check (plt > 5000)";
	string createTableRadProj2Command = ""
		+ "create table radproj2 as select mbr, spr, brc "
		+ "from radproj where spr=10";

	ExecuteSimple(createTableRadnik2Command);
	ExecuteSimple(alterTableRadnik2Command);
	ExecuteSimple(createTableRadProj2Command); 
}

void DropTempTables() 
{
	string dropTableRadnik2Command = "drop table radnik2";
	string dropTableRadProj2Command = "drop table radproj2";

	ExecuteSimple(dropTableRadnik2Command);
	ExecuteSimple(dropTableRadProj2Command);
}

bool ExecuteSimple(String sql)
{
	using (IDbConnection connection = Connection.ConnectionUtil.GetConnection())
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
					return true;
				}
				catch (DbException ex)
				{
					Console.WriteLine();
					Console.WriteLine("DML/DDL command failed. Exception:");
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
