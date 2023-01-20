using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Concepts.Connection
{
    public class ConnectionParams
    {
        public static readonly string LOCAL_DATA_SOURCE = "//localhost:1521/xe";
        public static readonly string CLASSROOM_DATA_SOURCE = "//192.168.7.204:1521/ubp";

        public static readonly string USER_ID = "username";
        public static readonly string PASSWORD = "password";
    }
}
