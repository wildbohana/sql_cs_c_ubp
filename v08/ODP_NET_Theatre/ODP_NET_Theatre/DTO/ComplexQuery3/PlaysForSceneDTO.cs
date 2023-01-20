using ODP_NET_Theatre.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODP_NET_Theatre.DTO
{
    public class PlaysForSceneDTO
    {
        public Scene Scene { get; set; }
        public List<PlayStatsDTO> Plays { get; set; } = new List<PlayStatsDTO>();

        public PlaysForSceneDTO(Scene scene)
        {
            this.Scene = scene;
        }
    }
}
