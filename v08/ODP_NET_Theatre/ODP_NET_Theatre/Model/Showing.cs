using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.Model
{
    public class Showing
    {
        public int OrdNumSh { get; set; }
        public DateTime DateSh { get; set; }
        public DateTime TimeSh { get; set; }
        public int NumOfSpecSh { get; set; }
        public int PlayIdPl { get; set; }
        public int SceneIdSc { get; set; }

        public Showing() { }

        public Showing(int brojGled, int playIdPl)
        {
            this.NumOfSpecSh = brojGled;
            this.PlayIdPl = playIdPl;
        }

        public Showing(int ordNumSh, DateTime dateSh, DateTime timeSh, int brojGled, int playIdPl, int idSc)
        {
            this.OrdNumSh = ordNumSh;
            this.DateSh = dateSh;
            this.TimeSh = timeSh;
            this.NumOfSpecSh = brojGled;
            this.PlayIdPl = playIdPl;
            this.SceneIdSc = idSc;
        }

        public override string ToString()
        {
            return string.Format("{0,-10} {1,-20:d} {2,-20:t} {3,-15} {4,-10} {5,-12}",
                OrdNumSh, DateSh, TimeSh, NumOfSpecSh, PlayIdPl, SceneIdSc);
        }

        public static string GetFormattedHeader()
        {
            return string.Format("{0,-10} {1,-20} {2,-20} {3,-15} {4,-10} {5,-12}",
                "ORD_NUM_SH", "DATE_SH", "TIME_SH", "NUM_OF_SPEC_SH", "PLAY_ID_PL", "SCENE_ID_SC");
        }

        public override bool Equals(object obj)
        {
            var showing = obj as Showing;

            return showing != null &&
                   OrdNumSh == showing.OrdNumSh &&
                   DateSh == showing.DateSh &&
                   TimeSh == showing.TimeSh &&
                   NumOfSpecSh == showing.NumOfSpecSh &&
                   PlayIdPl == showing.PlayIdPl &&
                   SceneIdSc == showing.SceneIdSc;
        }

        public override int GetHashCode()
        {
            var hashCode = 1323325588;

            hashCode = hashCode * -1521134295 + OrdNumSh.GetHashCode();
            hashCode = hashCode * -1521134295 + DateSh.GetHashCode();
            hashCode = hashCode * -1521134295 + TimeSh.GetHashCode();
            hashCode = hashCode * -1521134295 + NumOfSpecSh.GetHashCode();
            hashCode = hashCode * -1521134295 + PlayIdPl.GetHashCode();
            hashCode = hashCode * -1521134295 + SceneIdSc.GetHashCode();

            return hashCode;
        }
    }
}
