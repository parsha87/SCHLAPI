using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("StatusDataRTUDetails")]
    public partial class StatusDataRtudetails
    {
        public int? FrameId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime TimeStamp { get; set; }
        public int PrjId { get; set; }
        [Column("RTUIdinNW")]
        public int RtuidinNw { get; set; }
        public int SlotId { get; set; }
        [Column("RFSignalStrength")]
        public int? RfsignalStrength { get; set; }
        public int? NoOfHopes { get; set; }
        [Column("HH")]
        public int? Hh { get; set; }
        [Column("MM")]
        public int? Mm { get; set; }
        [Column("RTUBatVoltage")]
        [StringLength(10)]
        public string RtubatVoltage { get; set; }
        [Column("RTUChargingStatus")]
        public bool? RtuchargingStatus { get; set; }
        [Column("RTUAuxSense")]
        public bool? RtuauxSense { get; set; }
        [Column("RTUBoosterVoltage")]
        [StringLength(10)]
        public string RtuboosterVoltage { get; set; }
        public int NetworkId { get; set; }
    }
}
