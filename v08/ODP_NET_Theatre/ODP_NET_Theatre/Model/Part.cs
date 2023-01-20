using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.Model
{
    public class Role
    {
        public int RoleId { get; set; }
        public string NamePt { get; set; }
        public string GenderPt { get; set; }
        public string TypePt { get; set; }
        private int PlayIdPl { get; set; }

        public Role() { }

        public Role(int roleId, string namePt, string genderPt, string typePt, int playIdPl)
        {
            this.RoleId = roleId;
            this.NamePt = namePt;
            this.GenderPt = genderPt;
            this.TypePt = typePt;
            this.PlayIdPl = playIdPl;
        }

        public override string ToString()
        {
            return string.Format("{0,-8} {1,-12} {2,-10} {3,-10} {4,-10}",
                RoleId, NamePt, GenderPt, TypePt, PlayIdPl);
        }

        public static string GetFormattedHeader()
        {
            return string.Format("{0,-8} {1,-12} {2,-10} {3,-10} {4,-10}",
                "PART_ID", "name_ro", "gender_ro", "type_ro", "PLAY_ID_PL");
        }

        public override bool Equals(object obj)
        {
            var role = obj as Role;

            return role != null &&
                   RoleId == role.RoleId &&
                   NamePt == role.NamePt &&
                   GenderPt == role.GenderPt &&
                   TypePt == role.TypePt &&
                   PlayIdPl == role.PlayIdPl;
        }

        public override int GetHashCode()
        {
            var hashCode = 1742936682;

            hashCode = hashCode * -1521134295 + RoleId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NamePt);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(GenderPt);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TypePt);
            hashCode = hashCode * -1521134295 + PlayIdPl.GetHashCode();

            return hashCode;
        }
    }
}
