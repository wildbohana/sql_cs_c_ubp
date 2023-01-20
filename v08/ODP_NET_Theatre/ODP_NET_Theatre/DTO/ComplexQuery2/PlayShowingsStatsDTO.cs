using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.DTO
{
    public class PlayShowingsStatsDTO
    {
        public int PlayIdPl { get; set; }
        public int SpectatorsTotal { get; set; }
        public float SpectatorsAverage { get; set; }
        public int ShowingsTotal { get; set; }
               
        public PlayShowingsStatsDTO(int playIdPl, int spectatorsTotal, float spectatorsAverage, int showingsTotal)
        {
            this.PlayIdPl = playIdPl;
            this.SpectatorsTotal = spectatorsTotal;
            this.SpectatorsAverage = spectatorsAverage;
            this.ShowingsTotal = showingsTotal;
        }

        public override string ToString()
        {
            return string.Format("{0,-30} {1,-30:F2} {2,-30}", SpectatorsTotal, SpectatorsAverage, ShowingsTotal);
        }
    }
}
