using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.Model
{
    public class Scene
    {
        public int IdSc { get; set; }
        public string NameSc { get; set; }
        private int NumOfSeatsSc { get; set; }
        private int TheatreIdTh { get; set; }

        public Scene() { }

        public Scene(int idSc, string nameSc, int numOfSeatsSc, int theatreIdTh)
        {
            this.IdSc = idSc;
            this.NameSc = nameSc;
            this.NumOfSeatsSc = numOfSeatsSc;
            this.TheatreIdTh = theatreIdTh;
        }

        public Scene(string nameSc, int numOfSeatsSc, int theatreIdTh)
        {
            this.NameSc = nameSc;
            this.NumOfSeatsSc = numOfSeatsSc;
            this.TheatreIdTh = theatreIdTh;
        }

        public Scene(string nameSc, int numOfSeatsSc)
        {
            this.NameSc = nameSc;
            this.NumOfSeatsSc = numOfSeatsSc;
        }

        public override string ToString()
        {
            return string.Format("{0,-6} {1,-35} {2,-16} {3,-15}",
                IdSc, NameSc, NumOfSeatsSc, TheatreIdTh);
        }

        public static string GetFormattedHeader()
        {
            return string.Format("{0,-6} {1,-35} {2,-16} {3,-15}",
                "ID_SC", "NAME_SC", "NUM_OF_SEATS_SC", "THEATRE_ID_TH");
        }

        public override bool Equals(object obj)
        {
            var scene = obj as Scene;

            return scene != null &&
                   IdSc == scene.IdSc &&
                   NameSc == scene.NameSc &&
                   NumOfSeatsSc == scene.NumOfSeatsSc &&
                   TheatreIdTh == scene.TheatreIdTh;
        }

        public override int GetHashCode()
        {
            var hashCode = 1993609293;

            hashCode = hashCode * -1521134295 + IdSc.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NameSc);
            hashCode = hashCode * -1521134295 + NumOfSeatsSc.GetHashCode();
            hashCode = hashCode * -1521134295 + TheatreIdTh.GetHashCode();

            return hashCode;
        }
    }
}
