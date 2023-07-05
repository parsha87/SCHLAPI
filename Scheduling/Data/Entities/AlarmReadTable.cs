using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class AlarmReadTable
    {
        [Key]
        public int ReadAlarmId { get; set; }
        [StringLength(50)]
        public string Event { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EventDateTime { get; set; }
        public int? AlarmLevel { get; set; }
        [StringLength(2)]
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EventAckDateTime { get; set; }
        [StringLength(10)]
        public string Acknowledge { get; set; }
        public int? ThresholdAlarmsId { get; set; }
        [Column("RTUId")]
        public int? Rtuid { get; set; }
        public int? ChannelId { get; set; }
        public double? Value { get; set; }
        public int? ThresholdLimit { get; set; }
        public int? NetworkId { get; set; }
        [StringLength(100)]
        public string Reason { get; set; }
        public double? ActualThresholdValue { get; set; }
        [Column("IsSMSSend")]
        public bool? IsSmssend { get; set; }
        [StringLength(200)]
        public string TagName { get; set; }
        [StringLength(200)]
        public string PumpNo { get; set; }
    }
}
