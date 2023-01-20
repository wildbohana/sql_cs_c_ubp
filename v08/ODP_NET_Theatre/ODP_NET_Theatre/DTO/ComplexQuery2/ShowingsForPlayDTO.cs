using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODP_NET_Theatre.Model;

namespace ODP_NET_Theatre.DTO
{
    public class ShowingsForPlayDTO
    {
        public Play Play { get; set; }
        public PlayShowingsStatsDTO Stats { get; set; }
        public List<Showing> Showings { get; set; }
    }
}
