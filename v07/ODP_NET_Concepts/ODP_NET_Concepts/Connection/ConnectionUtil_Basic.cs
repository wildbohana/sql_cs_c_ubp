using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using System.Data.OracleClient;       // ne koristiti ovaj namespace, već ovaj ispod!
using Oracle.ManagedDataAccess.Client;  // (potrebno je skinuti paket Oracle.ManagedDataAccess sa nugeta)

namespace ODP_NET_Concepts.Connection
{
    public class ConnectionUtil_Basic : IDisposable
    {
        private static IDbConnection instance = null;

        public static IDbConnection GetConnection()
        {
            if (instance == null || instance.State == System.Data.ConnectionState.Closed)
            {
                OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
                ocsb.DataSource = Connection.ConnectionParams.LOCAL_DATA_SOURCE;
                ocsb.UserID = Connection.ConnectionParams.USER_ID;
                ocsb.Password = Connection.ConnectionParams.PASSWORD;
                instance = new OracleConnection(ocsb.ConnectionString);
            }
            return instance;
        }

        public void Dispose()
        {
            Console.WriteLine("Closing connection");
            if (instance != null)
            {
                instance.Close();
                instance.Dispose();
            }
        }
    }
}
