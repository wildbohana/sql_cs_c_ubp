using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Concepts
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Action> examples = new List<Action> {
                Command.Example01_Query.Example,                    //  0
                Command.Example02_QueryElegant.Example,             //  1
                Command.Example03_QueryWithParams.Example,          //  2
                Command.Example04_DDL_DML_QL.Example,               //  3
                Command.Example05_SQLInjection.Example,             //  4
                PreparedCommand.Example01_Query.Example,            //  5
                PreparedCommand.Example02_QueryWithParams.Example,  //  6
                PreparedCommand.Example03_SQLInjection.Example,     //  7
                PreparedCommand.Example04_DDL_DML_QL.Example,       //  8
                StoredProcedure.Example01_ExecuteFunction.Example,  //  9
                InMemoryDataSet.Example01_Iterating.Example,        //  10
                InMemoryDataSet.Example02_Updating.Example,         //  11
                Transaction.Example01_AutoCommit.Example,           //  12
                Transaction.Example02_ManualCommit.Example,         //  13
                ConnectionPool.Example01_ConnectionPool.Example,    //  14
            };

            examples[0].Invoke();

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
