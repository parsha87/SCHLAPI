using Scheduling.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class UpdateIdProjSchViewModel
    {
        public int ProjUpId { get; set; }
        public int MaxSchUpId { get; set; }

        public List<GatewayMaxSch> GatewayMaxSches { get; set; } = new List<GatewayMaxSch>();

}
}
