using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class EventViewModel
    {
        public string ObjName { get; set; }
        public string action { get; set; }
        public int prjId { get; set; }
        public int networkId { get; set; }
        public int objIdinDB { get; set; }
        public DateTime TimeZoneDateTime { get; set; }
    }
}
