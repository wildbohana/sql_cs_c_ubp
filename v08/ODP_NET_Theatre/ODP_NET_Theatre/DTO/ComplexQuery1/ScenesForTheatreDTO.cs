using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODP_NET_Theatre.Model;

namespace ODP_NET_Theatre.DTO
{
    public class ScenesForTheatreDTO
    {
        public Theatre Theatre { get; set; }
        public List<Scene> Scenes { get; set; }
    }
}
