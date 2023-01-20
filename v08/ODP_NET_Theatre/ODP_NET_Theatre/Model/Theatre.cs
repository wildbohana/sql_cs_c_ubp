using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.Model
{
    public class Theatre
    {
        public int IdTh { get; set; }
        public string NameTh { get; set; }
        public string AddressTh { get; set; }
        public string WebsiteTh { get; set; }
        public int PlaceIdPl { get; set; }

        public Theatre() { }

        public Theatre(int idTh, string nameTh, string addressTh, string websiteTh, int placeIdPl)
        {
            this.IdTh = idTh;
            this.NameTh = nameTh;
            this.WebsiteTh = websiteTh;
            this.AddressTh = addressTh;
            this.PlaceIdPl = placeIdPl;
        }

        public override string ToString()
        {
            return string.Format("{0,-6} {1,-35} {2,-20} {3,-35} {4,-30}",
                IdTh, NameTh, AddressTh, WebsiteTh, PlaceIdPl);
        }

        public static string GetFormattedHeader()
        {
            return string.Format("{0,-6} {1,-35} {2,-20} {3,-35} {4,-30}",
                "ID_TH", "NAME_TH", "ADDRESS_TH", "WEBSITE_TH", "PLACE_ID_PL");
        }

        public override bool Equals(object obj)
        {
            var theatre = obj as Theatre;

            return theatre != null &&
                   IdTh == theatre.IdTh &&
                   NameTh == theatre.NameTh &&
                   AddressTh == theatre.AddressTh &&
                   WebsiteTh == theatre.WebsiteTh &&
                   PlaceIdPl == theatre.PlaceIdPl;
        }

        public override int GetHashCode()
        {
            var hashCode = -1449613140;

            hashCode = hashCode * -1521134295 + IdTh.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NameTh);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AddressTh);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WebsiteTh);
            hashCode = hashCode * -1521134295 + EqualityComparer<int>.Default.GetHashCode(PlaceIdPl);

            return hashCode;
        }
    }
}
