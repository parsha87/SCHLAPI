using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class SequenceShortViewModel
    {
        public int SeqId { get; set; }
        public int PrjId { get; set; }
        public int PrgId { get; set; }
        public int NetworkId { get; set; }
        public int ZoneId { get; set; }
        public DateTime? SeqStartDate { get; set; }
        public DateTime? SeqEndDate { get; set; }
        public string BasisOfOp { get; set; }
        public string SeqType { get; set; }
        public string SeqTagName { get; set; }
        public string NetworkName { get; set; }
        public string ZoneName { get; set; }
        public string SeqName { get; set; }
        public string SeqMasterStartTime { get; set; }
        public bool IsValid { get; set; }
        public int? IntervalDays { get; set; }

        public int? SeqNo { get; set; }
        public List<int> WeekDays { get; set; } = new List<int>();
    }
}
