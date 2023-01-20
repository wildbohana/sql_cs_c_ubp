using ODP_NET_Theatre.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.DTO
{
    public class PlayDTO
    {
        public int IdPl { get; set; }
        public string NamePl { get; set; }
        public double SpectatorsAverage { get; set; }
        
        public int FemaleRolesTotal { get; set; }
        public int MaleRolesTotal { get; set; }

        public List<Role> Roles { get; set; } = new List<Role>();

        public PlayDTO(int idPl, string namePl, double spectatorsAverage)
        {
            this.IdPl = idPl;
            this.NamePl = namePl;
            this.SpectatorsAverage = spectatorsAverage;
        }

        public static string GetFormatedHeader()
        {
            return string.Format("{0,-8} {1,-20} {2,-20}", "IDPRE", "DNAZIV", "PROSECAN_BR_GLEDALACA");
        }

        public override string ToString()
        {
            return string.Format("{0,-8} {1,-20} {2,-20:F2} ",
                IdPl, NamePl, SpectatorsAverage);
        }
    }
}
