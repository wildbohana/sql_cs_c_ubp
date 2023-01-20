using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODP_NET_Concepts.Util;
using Oracle.ManagedDataAccess.Client;

namespace ODP_NET_Concepts.StoredProcedure
{
    public static class Example01_ExecuteFunction
    {
        public static void Example()
        {
            // spr i brc vrednosti za koje prebrojavamo radnike
            int spr = 30;
            int brc = 3;

            using (IDbConnection connection = Connection.ConnectionUtil_Basic.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "F_SEL_RadprojCnt";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        
                        // Prvi parametar komande obavezno mora biti povratna vrednost
                        ParameterUtil.AddParameter(command, "returnValue",  DbType.Int32, System.Data.ParameterDirection.ReturnValue);
                        ParameterUtil.AddParameter(command, "spr",          DbType.Int32, System.Data.ParameterDirection.Input);
                        ParameterUtil.AddParameter(command, "brc",          DbType.Int32, System.Data.ParameterDirection.Input);

                        ParameterUtil.SetParameterValue(command, "spr", spr);
                        ParameterUtil.SetParameterValue(command, "brc", brc);
                        command.ExecuteNonQuery();

                        int result = int.Parse(ParameterUtil.GetParameterValue(command, "returnValue").ToString());

                        Console.WriteLine("Broj radnika koji na projektu sa šifrom {0} rade više od {1} sata je: {2}.", spr, brc, result);
                    }
                }
                catch (OracleException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
