using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiSensorAlarmData
    {
        [Key]
        public int Id { get; set; }
        public int? FrameType { get; set; }
        public int? NodeId { get; set; }
        [Column("SOY")]
        public string Soy { get; set; }
        public int? AlarmType { get; set; }
        public int? SensorNo { get; set; }
        public int? SensorType { get; set; }
        public int? AlarmReason { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? SensorValue { get; set; }
        [Column("CState")]
        public int? Cstate { get; set; }
        public int? DefaultState { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? CummulativeCount { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? Frequency { get; set; }
        public int? Reserved { get; set; }
        [Column("ucaLdata")]
        [StringLength(50)]
        public string UcaLdata { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AddedDateTime { get; set; }
        public int? Gwsoy { get; set; }
        public int? NodeNo { get; set; }
        public int? NetworkNo { get; set; }
        public int? GatewayId { get; set; }
    }
}
