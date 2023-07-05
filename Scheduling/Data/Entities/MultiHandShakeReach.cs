using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiHandShakeReach
    {
        [Key]
        public int Id { get; set; }
        public int? FrameType { get; set; }
        [Column("NodeID")]
        public int? NodeId { get; set; }
        [Column("SOY")]
        public string Soy { get; set; }
        public int? VerNumber { get; set; }
        public int? ConfigVerNumber { get; set; }
        [Column("ScheduleUPNo")]
        public int? ScheduleUpno { get; set; }
        [Column("SensorUPNo")]
        public int? SensorUpno { get; set; }
        [Column("FbwConfigUPNo")]
        public int? FbwConfigUpno { get; set; }
        public int? TempReading { get; set; }
        public int? BatteryVoltReading { get; set; }
        public int? LvsStartTime { get; set; }
        public int? LvsDuration { get; set; }
        public int? LvsOperation { get; set; }
        public int? LvsValveNo { get; set; }
        public int? LvsRop { get; set; }
        public int? ChargingStatus { get; set; }
        public int? ValveStatus { get; set; }
        [Column("DIStatus")]
        public int? Distatus { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AddedDatetime { get; set; }
        public int? NetworkNo { get; set; }
        public int? NodeNo { get; set; }
        public int? GatewayId { get; set; }
    }
}
