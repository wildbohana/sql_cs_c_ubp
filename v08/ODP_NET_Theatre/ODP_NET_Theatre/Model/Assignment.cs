using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.Model
{
    public class Assignment
    {
        public int IdAs { get; set; }
        public double HonorarAs { get; set; }
        public DateTime StartDateAs { get; set; }
        public DateTime EndDateAs { get; set; }
        public string RoleIdPt { get; set; }
        public int ActorIdAc { get; set; }

        public Assignment() { }

        public Assignment(int idAs, double honorarAs, DateTime startDateAs, DateTime endDateAs, string roleIdPt, int actorIdAc)
        {
            this.IdAs = idAs;
            this.HonorarAs = honorarAs;
            this.StartDateAs = startDateAs;
            this.EndDateAs = endDateAs;
            this.RoleIdPt = roleIdPt;
            this.ActorIdAc = actorIdAc;
        }
        
        public override bool Equals(object obj)
        {
            var assignment = obj as Assignment;

            return assignment != null &&
                   IdAs == assignment.IdAs &&
                   HonorarAs == assignment.HonorarAs &&
                   StartDateAs == assignment.StartDateAs &&
                   EndDateAs == assignment.EndDateAs &&
                   RoleIdPt == assignment.RoleIdPt &&
                   ActorIdAc == assignment.ActorIdAc;
        }

        public override int GetHashCode()
        {
            var hashCode = 202752905;

            hashCode = hashCode * -1521134295 + IdAs.GetHashCode();
            hashCode = hashCode * -1521134295 + HonorarAs.GetHashCode();
            hashCode = hashCode * -1521134295 + StartDateAs.GetHashCode();
            hashCode = hashCode * -1521134295 + EndDateAs.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RoleIdPt);
            hashCode = hashCode * -1521134295 + ActorIdAc.GetHashCode();

            return hashCode;
        }
    }
}
