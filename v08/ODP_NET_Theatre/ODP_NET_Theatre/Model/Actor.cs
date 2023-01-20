using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.Model
{
    public class Actor
    {
        public int IdAc { get; set; }
        public string NameAc { get; set; }
        public DateTime DobAc { get; set; }
        public bool StatusAc { get; set; }
        public double SalaryAc { get; set; }
        public double BonusAc { get; set; }
        public int TheatreIdTh { get; set; }

        public Actor(int idAc, string nameAc, DateTime dobAc, bool statusAc, double salaryAc, double bonusAc, int idTh)
        {
            this.IdAc = idAc;
            this.NameAc = nameAc;
            this.DobAc = dobAc;
            this.StatusAc = statusAc;
            this.SalaryAc = salaryAc;
            this.BonusAc = bonusAc;
            this.TheatreIdTh = idTh;
        }
        
        public override bool Equals(object obj)
        {
            var actor = obj as Actor;

            return actor != null &&
                   IdAc == actor.IdAc &&
                   NameAc == actor.NameAc &&
                   DobAc == actor.DobAc &&
                   StatusAc == actor.StatusAc &&
                   SalaryAc == actor.SalaryAc &&
                   BonusAc == actor.BonusAc &&
                   TheatreIdTh == actor.TheatreIdTh;
        }

        public override int GetHashCode()
        {
            var hashCode = 314861883;

            hashCode = hashCode * -1521134295 + IdAc.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NameAc);
            hashCode = hashCode * -1521134295 + DobAc.GetHashCode();
            hashCode = hashCode * -1521134295 + StatusAc.GetHashCode();
            hashCode = hashCode * -1521134295 + SalaryAc.GetHashCode();
            hashCode = hashCode * -1521134295 + BonusAc.GetHashCode();
            hashCode = hashCode * -1521134295 + TheatreIdTh.GetHashCode();

            return hashCode;
        }
    }
}
