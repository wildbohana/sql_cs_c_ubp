using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.DTO
{
    public class ShowingDeleteDTO
    {
        public string NameSc { get; set; }
        public int NumOfSeatsSc { get; set; }
        public int TheatreIdTh { get; set; }
        public int SceneIdSc { get; set; }
        public int OrdNumSh { get; set; }
        public DateTime DateSh { get; set; }
        public DateTime TimeSh { get; set; }
        public int NumOfSpecSh { get; set; }
        public int PlayIdPl { get; set; }

        public ShowingDeleteDTO() { }

        public ShowingDeleteDTO(string nameSc, int numOfSeatsSc, int theatreIdTh, int sceneIdSc, int ordNumSh, DateTime dateSh, DateTime timeSh, int numOfSpecSh, int playIdPl)
        {
            NameSc = nameSc;
            NumOfSeatsSc = numOfSeatsSc;
            TheatreIdTh = theatreIdTh;
            SceneIdSc = sceneIdSc;
            OrdNumSh = ordNumSh;
            DateSh = dateSh;
            TimeSh = timeSh;
            NumOfSpecSh = numOfSpecSh;
            PlayIdPl = playIdPl;
        }

        public override string ToString()
        {
            return string.Format("{0,-20} {1,-15} {2,-10} {3,-12} {4,-10} {5,-10} {6,-10} {7,-10} {8,-10}",
                NameSc, NumOfSeatsSc, TheatreIdTh, SceneIdSc, OrdNumSh, DateSh.ToString("d"), TimeSh.ToString("t"), NumOfSpecSh, PlayIdPl);
        }

        public static string GetFormattedHeader()
        {
            return string.Format("{0,-20} {1,-15} {2,-10} {3,-12} {4,-10} {5,-10} {6,-10} {7,-10} {8,-10}",
                "NAME_sc", "NUM_OF_SEATS_SC", "THEATRE_ID", "SCENE_ID_SC", "ORD_NUM_SH", "DATE_SH", "TIME_SH", "NUM_OF_SPEC_SH", "PLAY_ID_PL");
        }
    }
}
