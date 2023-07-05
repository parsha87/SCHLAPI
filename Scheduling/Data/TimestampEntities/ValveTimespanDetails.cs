using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.TimestampEntities
{
    public partial class ValveTimespanDetails
    {
        [Key]
        public int ValveTimespanId { get; set; }
        public int? SeqId { get; set; }
        public int? PrjId { get; set; }
        public int? PrgId { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        public int? ChannelId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SeqDate { get; set; }
        public int? StartTimeSpanId { get; set; }
        public int? EndTimeSpanId { get; set; }
        public int? LoopingSeqId { get; set; }
        public int? IsCallAlert { get; set; }
        public int? IsSchAlert { get; set; }
    }
}
