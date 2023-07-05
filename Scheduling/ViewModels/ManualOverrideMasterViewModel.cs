using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class ManualOverrideMasterViewModel
    {
        public int Moid { get; set; }
        public int? Motypeid { get; set; }
        public string UserName { get; set; }
        public DateTime? MocreatedDate { get; set; }
        public int? OverrideForId { get; set; }
        public string OverrideForText { get; set; }
        public int? ActionTypeId { get; set; }
        public int? AlarmLevel { get; set; }
        public string Status { get; set; }
        public DateTime? ExecutionDatetime { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        public int? BlockId { get; set; }
        public DateTime? MostartDateTime { get; set; }
        public DateTime? MoendDateTime { get; set; }
        public string Moname { get; set; }
        public bool? IsDeleted { get; set; }
        public string TagName { get; set; }
        public bool? MasterValveOpEnabled { get; set; }
        public bool? IsSmssend { get; set; }
        public List<ElementNo> Elements { get; set; } = new List<ElementNo>();


    }

    public class Elements
    {
        public int ElementNo { get; set; }
        public string ElementName { get; set; }
    }
}
