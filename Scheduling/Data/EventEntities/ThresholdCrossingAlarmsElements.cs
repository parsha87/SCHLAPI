using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class ThresholdCrossingAlarmsElements
    {
        public int? ThresholdAlarmsId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AlarmDate { get; set; }
        [Column("RTUId")]
        public int? Rtuid { get; set; }
        public int? ChannelId { get; set; }
        public int? Value { get; set; }
        [StringLength(50)]
        public string AlarmStatus { get; set; }
        [StringLength(200)]
        public string PumpNo { get; set; }
    }
}
