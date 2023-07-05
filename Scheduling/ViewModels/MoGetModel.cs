using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class MOGetDataModel
    {
        public int MOId { get; set; }
        public string Name { get; set; }
        public int MOTypeId { get; set; }
        public string MOType { get; set; }
        public string userid { get; set; }
        public string Username { get; set; }        
        public string MocreatedDate { get; set; }
        public int? OverrideForId { get; set; }
        public string Objectname { get; set; }
        public int? ActionTypeId { get; set; }
        public string Action { get; set; }
        public int? alarmLevel { get; set; }
        public string Status { get; set; }
        public string statusDB { get; set; }
        public string Target { get; set; }
        public string ExecutionDatetime { get; set; }        
    }
}
