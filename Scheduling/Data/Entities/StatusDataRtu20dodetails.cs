using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("StatusDataRTU20DODetails")]
    public partial class StatusDataRtu20dodetails
    {
        [Key]
        public int Id { get; set; }
        public int? FrameId { get; set; }
        public int? NetworkId { get; set; }
        [Column("RTUTimeStamp", TypeName = "datetime")]
        public DateTime? RtutimeStamp { get; set; }
        [Column("HH")]
        public int? Hh { get; set; }
        [Column("MM")]
        public int? Mm { get; set; }
        [Column("RTUIdInNW")]
        public int? RtuidInNw { get; set; }
        [Column("SlotIdinRTU")]
        public int? SlotIdinRtu { get; set; }
        public int? EnableDisable { get; set; }
        public int? SignalStrength { get; set; }
        [Column("HOPCounter")]
        public int? Hopcounter { get; set; }
        [Column("version")]
        public int? Version { get; set; }
        public int? Corruptversion { get; set; }
        public int? BatteryVoltage { get; set; }
        [Column("NetworkLOSTCount")]
        public int? NetworkLostcount { get; set; }
        [Column("ACK")]
        public int? Ack { get; set; }
        public int? TheftDetect { get; set; }
        public int? ReqvalveOperation { get; set; }
        public int? ReasonId { get; set; }
        public int? ValveDecision { get; set; }
        public int? ValveStatus { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ReceivedDateTime { get; set; }
    }
}
