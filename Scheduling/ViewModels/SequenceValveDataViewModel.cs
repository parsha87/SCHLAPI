using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class SequenceValveDataViewModel
    {
        public int? MstseqId { get; set; }
        public int? SeqId { get; set; }
        public int? ProjectId { get; set; }
        public int? PrgId { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        public int? StartId { get; set; }
        public string ValveStartTime { get; set; }
        public int? SeqNo { get; set; }
        public int TimeSpanId { get; set; }
        public string GroupName { get; set; }
        public string Valve { get; set; }
        public int? ChannelId { get; set; }
        public string ValveStartDuration { get; set; }

        public bool? IsFertilizerRelated { get; set; }
        public bool? IsFlushRelated { get; set; }

        public int? ScheduleNo { get; set; }

    }
}
