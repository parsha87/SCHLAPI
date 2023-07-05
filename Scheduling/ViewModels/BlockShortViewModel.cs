using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class BlockShortViewModel
    {
        public int BlockId { get; set; }
        public int? ZoneId { get; set; }
        public int? NetworkId { get; set; }
        public string Name { get; set; }
    }
}
