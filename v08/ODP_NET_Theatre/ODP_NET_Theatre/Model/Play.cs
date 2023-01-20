using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.Model
{
    public class Play
    {
        public int IdPl { get; set; }
        public string NamePl { get; set; }
        public string DurationPl { get; set; }
        public int YearPl { get; set; }

        public Play() { }

        public Play(int playIdPl)
        {
            this.IdPl = playIdPl;
        }

        public Play(int idPl, string namePl, string durationPl, int yearPl)
        {
            this.IdPl = idPl;
            this.NamePl = namePl;
            this.DurationPl = durationPl;
            this.YearPl = yearPl;
        }

        public override string ToString()
        {
            return string.Format("{0,-6} {1,-30} {2,-10} {3,-10}",
                IdPl, NamePl, DurationPl, YearPl);
        }

        public static string GetFormattedHeader()
        {
            return string.Format("{0,-6} {1,-30} {2,-10} {3,-10}",
                "IDPRED", "NAZIVPRED", "TRAJANJE", "GODINAPRE");
        }

        public override bool Equals(object obj)
        {
            var play = obj as Play;

            return play != null &&
                   IdPl == play.IdPl &&
                   NamePl == play.NamePl &&
                   DurationPl == play.DurationPl &&
                   YearPl == play.YearPl;
        }

        public override int GetHashCode()
        {
            var hashCode = 1123982895;

            hashCode = hashCode * -1521134295 + IdPl.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NamePl);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DurationPl);
            hashCode = hashCode * -1521134295 + YearPl.GetHashCode();

            return hashCode;
        }
    }
}
