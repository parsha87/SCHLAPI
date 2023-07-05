using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class ProgramViewModel
    {
        public int? ProgIndId { get; set; }
        public string Name { get; set; }
        public int? PrjId { get; set; }
        public int? NetworkId { get; set; }
        public int? ProgId { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public bool? IsLooping { get; set; }
        public bool? IsLocked { get; set; }

    }
}
