using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class FilterValveGroupConfigViewModel
    {
        public int MstfilterGroupId { get; set; }
        public int? ProjectId { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        public int? Rtuid { get; set; }
        public int? OpGroupTypeId { get; set; }
        public string GroupName { get; set; }
        public int? OperationType { get; set; }
        public bool? PauseWhileFlush { get; set; }
        public int? MaxReiterationThroughPd { get; set; }
        public int? ActionAfterMaxIteration { get; set; }
        public int? OffsetForFilterFlushinMin { get; set; }
        public int? FlushTimeinMin { get; set; }
        public int? DelayBetweenFlushinSec { get; set; }
        public int? WaterMeterNumber { get; set; }
        public int? RuleNoDirtSenseAlarm { get; set; }
        public int? PdsensorNo { get; set; }
        public int? StartSustainingBeforeFlush { get; set; }
        public int? Timedeviation { get; set; }
        public int? RuleNoWithPd { get; set; }
        public int? GrpId { get; set; }
        public string TagName { get; set; }

        public virtual List<FilterValveGroupElementsConfigViewModel> FilterValveGroupElementsConfig { get; set; }
    }
}
