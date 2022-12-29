void CreateTableFazeProjekta()
{
	string sql = ""
		+ "create table faze_projekta (spr int, sfp int, rukfp int, nafp varchar2(10) "
		+ "datp date, constraint fp_pk primary key (spr, sfp), constrint fp_fk "
		+ "foreign key (rukfp) references radnik(mbr))";

	using (IDbConnection connection = Connection.ConnectionUtil.GetConnection())
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

void SelectAll()
{
	string sql = "select spr, sfp, rukfp, nafp, datp from faze_projekta";

	using (IDbConnection connection = Connection.ConnectionUtil.GetConnection())
	{
		try 
		{
			connection.Open();
			using (IdbCommand command = connection.CreateCommand())
			{
				command.CommandText = sql;

				using (IDataReader reader = command.ExecuteReader())
				{
					Console.WriteLine("SPR: /t/tSFP: /t/t RUK: /t/t NAF: /t/t DAT: ");

					while (reader.Read())
					{
						string sbr = reader.IsDBNull(0) ? "" : reader.GetString(0);
						string sfp = reader.IsDBNull(1) ? "" : reader.GetString(1);
						string ruk = reader.IsDBNull(2) ? "" : reader.GetString(2);
						string naf = reader.IsDBNull(3) ? "" : reader.GetString(3);
						string dat = reader.IsDBNull(4) ? "" : reader.GetString(4);
						
						Console.WriteLine(sbr, sfp, ruk, naf, dat);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}

void SelectBySprAndSfp()
{
	Console.WriteLine("SPR: ");
	string spr = Console.ReadLine();
	Console.WriteLine("SFP: ");
	string sfp = Console.ReadLine();

	using (IDbConnection connection = Connection.ConnectionUtil.GetConnection())
	{
		try
		{
			connection.Open();
			using (IDbCommand command = connection.CreateComman())
			{
				command.CommandText = "select * from faze_projekta where spr = :spr and sfp = :sfp";

				ParametriUtil.AddParameter(command, "spr", DbType.Int32, 0);
				ParametriUtil.AddParameter(command, "sfp", DbType.Int32, 0);

				command.Prepare();

				ParameterUtil.SetParameterValue(command, "spr", spr);
				ParameterUtil.SetParameterValue(command, "sfp", sfp);

				using (IDataReader reader = command.ExecuteReader())
				{
					Console.WriteLine("SPR: /t/tSFP: /t/tRUK: /t/t NAF: /t/t DAT: ");

					if (((DataReader)reader).HasRows)
					{
						while (reader.Read())
						{
							string sbr = reader.IsDBNull(0) ? "" : reader.GetString(0);
							string sfp = reader.IsDBNull(1) ? "" : reader.GetString(1);
							string ruk = reader.IsDBNull(2) ? "" : reader.GetString(2);
							string naf = reader.IsDBNull(3) ? "" : reader.GetString(3);
							string dat = reader.IsDBNull(4) ? "" : reader.GetString(4);
							
							Console.WriteLine(sbr, sfp, ruk, naf, dat);
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

void InserRow()
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

	string sql = ""
		+ "insert into faze_projekta (spr, sfp, rukfp, nafp, datp) "
		+ "values (:spr, :sfp, :rukfo, :nafp, to_date(:datp, 'dd.MM.yyyy.'))";

	using (IDbConnection connection = Connection.ConnectionUtil.GetConnection())
	{
		try 
		{
			connection.Open();

			using (IDbCommand command = connection.CreateCommand())
			{
				command.CommandText = sql;
				
				ParametriUtil.AddParameter(command, "spr", DbType.Int32, 0);
				ParametriUtil.AddParameter(command, "sfp", DbType.Int32, 0);
				ParametriUtil.AddParameter(command, "rukfp", DbType.Int32, 0);
				ParametriUtil.AddParameter(command, "nafp", DbType.Int32, 0);
				ParametriUtil.AddParameter(command, "datp", DbType.String, 10);

				command.Prepare();

				ParametriUtil.SetParameterValue(command, "spr", string.IsNullOrEmpty(spr) ? null : spr);
				ParametriUtil.SetParameterValue(command, "sfp", string.IsNullOrEmpty(sfp) ? null : sfp);
				ParameterUtil.SetParameterValue(command, "rukfp", string.IsNullOrEmpty(rukfp) ? null : rukfp);
				ParameterUtil.SetParameterValue(command, "nafp", string.IsNullOrEmpty(nafp) ? null : nafp);
				ParameterUtil.SetParameterValue(command, "datp", string.IsNullOrEmpty(datp) ? null : datp);

				int rowsAffected = command.ExecuteNonQuery();
				if (rowsAffected > 0)
				{
					Console.WriteLine("Row sucessfully inserted");
				}
				else
				{
					Console.WriteLine("Insert failed!");
				}
			}
		}
		catch (DbException ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}

void ModifyRow()
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

	string sql = ""
		+ "update faze_projekta set rukfp=:rukfp, nafp=:nafp, "
		+ "datp=to_date(:datp, 'dd.MM.yyyy.') where spr=:spr and sfp=:sfp";
	
	using (IDbConnection connection = Connection.ConnectionUtil.GetConnection())
	{
		try
		{
			connection.Open();

			using (IDbCommand command = connection.CreateCommand())
			{
				command.CommandText = sql;

				// Redosled parametara u skladu sa sql query naredbom
				ParametriUtil.AddParameter(command, "rukfp", DbType.Int32, 0);
				ParameterUtil.AddParameter(command, "nafp", DbType.String, 20);
				ParameterUtil.AddParameter(command, "datp", DbType.String, 10);
				ParameterUtil.AddParameter(command, "spr", DbType.Int32, 0);
				ParameterUtil.AddParameter(command, "sfp", DbType.Int32, 0);

				command.Prepare();

				ParametriUtil.SetParameterValue(command, "spr", string.IsNullOrEmpty(spr) ? null : spr);
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

void DeleteRow()
{
	Console.WriteLine("SPR: ");
	string spr = Console.ReadLine();
	Console.WriteLine("SFP: ");
	string sfp = Console.ReadLine();

	string sql = "delete from faze_projekta where spr=:spr and sfp=:sfp";

	using (IDbConnection connection = Connection.ConnectionUtil.GetConnection())
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

void DropTableFazeProjekta()
{
	string sql = "drop table faze_projekta";

	using (IDbConnection connection = Connection.ConnectionUtil.GetConnection())
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
