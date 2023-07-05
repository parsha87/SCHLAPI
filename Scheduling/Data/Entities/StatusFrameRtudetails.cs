using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("StatusFrameRTUDetails")]
    public partial class StatusFrameRtudetails
    {
        [Key]
        [Column(TypeName = "datetime")]
        public DateTime TimeStamp { get; set; }
        [Key]
        public int PrjId { get; set; }
        [Key]
        public int NetworkId { get; set; }
        [Key]
        [Column("RTUIdinNW")]
        public int RtuidinNw { get; set; }
        [Key]
        public int SlotId { get; set; }
        [Column("RFSignalStrength")]
        public int? RfsignalStrength { get; set; }
        public int? NoOfHopes { get; set; }
        [Column("HH")]
        public int? Hh { get; set; }
        [Column("MM")]
        public int? Mm { get; set; }
        [Column("RTUBatVoltage")]
        public int? RtubatVoltage { get; set; }
        [Column("RTUChargingStatus")]
        public bool? RtuchargingStatus { get; set; }
        [Column("RTUAuxSense")]
        public bool? RtuauxSense { get; set; }
        [Column("RTUBoosterVoltage")]
        public int? RtuboosterVoltage { get; set; }
    }
}
