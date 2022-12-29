// String zahteva za bazu
string query = "select mbr, ime, prz, plt from radnik";

// Ispis odgovora od baze
void DataSetIteracija()
{
	using (IDbConnection connection = ConnectionUtil_Basic.ConnectionUtil.GetConnection())
	{
		try
		{
			connection.Open();
			using (IDbCommand command = connection.CreateCommand())
			{
				command.CommandText = query;

				using (DbDataAdapter adapter = new OracleDataAdapter((OracleCommand)command))
				{
					DataSet dataSet = new DataSet();
					adapter.Fill(dataSet);

					DataTable dataTable = dataSet.Tables[0];

					Console.WriteLine("MBR:	/t/tIME: /t/tPRZ: /t/tPLT:");
					foreach (DataRow row in dataTable.Rows)
					{
						Console.WriteLine(row["mbr"], row["ime"], row["prz"], row["plt]"]);
					}
					Console.WriteLine();
				}
			}
		}
		catch (DbException ex)
		{
			Console.WriteLine(ex.Message);
		}
	}
}