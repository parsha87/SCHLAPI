using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiValveAlarmData
    {
        [Key]
        public int Id { get; set; }
        public int? ValveNo { get; set; }
        public int? ValveType { get; set; }
        public int? ReqState { get; set; }
        public int? ReqStateReason { get; set; }
        public int? CurrentState { get; set; }
        [Column("CStateReason")]
        public int? CstateReason { get; set; }
        public int? AlarmReason { get; set; }
        public string UpdateTime { get; set; }
        [Column("ACVCurrentConsimption")]
        public int? AcvcurrentConsimption { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AddedDateTime { get; set; }
        public int? FrameType { get; set; }
        public int? NodeId { get; set; }
        [Column("SOY")]
        public string Soy { get; set; }
        public int? AlarmType { get; set; }
        [Column("GWSoy")]
        public int? Gwsoy { get; set; }
        [Column("GWMoy")]
        public int? Gwmoy { get; set; }
        public int? NodeNo { get; set; }
        public int? NetworkNo { get; set; }
        public int? GatewayId { get; set; }
    }
}
