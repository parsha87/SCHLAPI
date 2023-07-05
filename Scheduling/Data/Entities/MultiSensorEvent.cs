using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiSensorEvent
    {
        [Key]
        public int Id { get; set; }
        public int? TotalBytes { get; set; }
        public int? FrameType { get; set; }
        public int? NodeId { get; set; }
        [Column("SSNO")]
        public int? Ssno { get; set; }
        [Column("SSType")]
        public int? Sstype { get; set; }
        [Column("SSPriority")]
        public int? Sspriority { get; set; }
        [Column("SAMPRate")]
        public int? Samprate { get; set; }
        public string LastAtmSoy { get; set; }
        [Column("UTSoy")]
        public string Utsoy { get; set; }
        public string StoreSoy { get; set; }
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
        public string UcaLdata { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AddedDateTime { get; set; }
        [Column("GWLastAtmSoy")]
        public int? GwlastAtmSoy { get; set; }
        [Column("GWUtsoy")]
        public int? Gwutsoy { get; set; }
        [Column("GWStoreSoy")]
        public int? GwstoreSoy { get; set; }
        public int? NodeNo { get; set; }
        public int? NetworkNo { get; set; }
        public int? GatewayId { get; set; }
    }
}
