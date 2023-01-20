using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.DTO
{
    public class PlayStatsDTO
    {
        public int PlayId { get; set; }
        public int SpectatorsTotal { get; set; }
        public int RolesTotal { get; set; }

        public PlayStatsDTO(int playId, int spectatorsTotal)
        {
            PlayId = playId;
            SpectatorsTotal = spectatorsTotal;
        }

        public override String ToString()
        {
            return String.Format("{0, -27} {1, -29.2F} {2, -17}", PlayId, SpectatorsTotal, RolesTotal);
        }
    }
}
