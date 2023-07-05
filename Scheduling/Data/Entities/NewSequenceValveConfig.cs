using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class NewSequenceValveConfig
    {
        [Key]
        public int SeqGrEleId { get; set; }
        public int? SeqId { get; set; }
        public int? StartId { get; set; }
        public int? HorizGrId { get; set; }
        [StringLength(50)]
        public string GroupName { get; set; }
        [StringLength(50)]
        public string Valve { get; set; }
        [StringLength(10)]
        public string ValveStartTime { get; set; }
        [StringLength(10)]
        public string ValveStartDuration { get; set; }
        public bool? IsFlushRelated { get; set; }
        public bool? IsFertilizerRelated { get; set; }
        public int? FertGrNo { get; set; }
        public int? FertGrSettingNo { get; set; }
        public int? Typeofoperation { get; set; }
        public int? TimeOfIrrigation { get; set; }
        public int? DurationOfFert { get; set; }
        [StringLength(10)]
        public string Unit { get; set; }
        public bool? IsHorizontal { get; set; }
        public int? ChannelId { get; set; }
        public int? ScheduleNo { get; set; }
    }
}
