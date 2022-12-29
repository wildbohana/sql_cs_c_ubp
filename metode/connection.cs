// Connection : GetConnection
IDbConnection GetConnection()
{
	if (instance == null || instance.State == System.Data.ConnectionState.Closed)
	{
		OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
		ocsb.DataSource = LOCAL_DATA_SOURCE;
		ocsb.UserID = USER_ID;
		ocsb.Password = PASSWORD;

		ocsb.Polling = true;
		ocsb.MinPoolSize = 1;
		ocsb.MaxPoolSize = 10;
		ocsb.IncrPoolSize = 3;
		ocsb.ConnectionLifeTime = 5;
		ocsb.ConnectionTimeout = 30;

		instance = new OracleConnection(ocsb.ConnectionString);
	}	
	return instance;
}

// Connection : Dispose
void Dispose()
{
	Console.WriteLine("Closing connection.");
	if (instance != null)
	{
		instance.Close();
		instance.Dispose();
	}
}
