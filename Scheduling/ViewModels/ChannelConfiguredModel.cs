using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class ChannelConfiguredModel
    {
        public int ValveTimespanId { get; set; }
        public int SeqId { get; set; }
        public int PrjId { get; set; }
        public int PrgId { get; set; }
        public int NetworkId { get; set; }
        public int ZoneId { get; set; }
        public int ChannelId { get; set; }
        public DateTime SeqDate { get; set; }
        public int StartTimeSpanId { get; set; }
        public int EndTimeSpanId { get; set; }
    }
}
