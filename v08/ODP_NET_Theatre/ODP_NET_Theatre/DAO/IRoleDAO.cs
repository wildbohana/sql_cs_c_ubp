using ODP_NET_Theatre.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.DAO
{
    public interface IRoleDAO : ICRUDDao<Role, int>
    {
        int FindCountByPlayId(int playIdPl);

        List<Role> FindRoleByPlayId(int idPl);

        int FindCountForRoleGender(int idPl, string gender);
    }
}
