using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("StatusDataRTULineDetails")]
    public partial class StatusDataRtulineDetails
    {
        public int? FrameId { get; set; }
        [Column("RTUIdInNW")]
        public int RtuidInNw { get; set; }
        [Column("RTUTimeStamp", TypeName = "datetime")]
        public DateTime RtutimeStamp { get; set; }
        public int ChannelId { get; set; }
        [Required]
        [StringLength(1)]
        public string AorDorV { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal AnalogValue { get; set; }
        public bool DigitalStatus { get; set; }
        public int DigitalCounterVal { get; set; }
        public int DigitalFlowVal { get; set; }
        public int PrjId { get; set; }
        public int NetworkId { get; set; }
        public int SlotId { get; set; }
        public int? ValveStatus { get; set; }
        [Column("CalculatedAValue")]
        [StringLength(50)]
        public string CalculatedAvalue { get; set; }
        [Column("CalculatedDCValue")]
        [StringLength(50)]
        public string CalculatedDcvalue { get; set; }
        [Column("CalculatedDFValue")]
        [StringLength(50)]
        public string CalculatedDfvalue { get; set; }
        public int? CalculatedSensorValue { get; set; }
    }
}
