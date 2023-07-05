using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("GWStatusData")]
    public partial class GwstatusData
    {
        [Key]
        public int Id { get; set; }
        public int? TotalBytes { get; set; }
        public int? FrameType { get; set; }
        public int? NodeId { get; set; }
        [Column("CTimeSoy")]
        public string CtimeSoy { get; set; }
        public int? ActiveSec { get; set; }
        [Column("GWVBat", TypeName = "decimal(18, 2)")]
        public decimal? Gwvbat { get; set; }
        [Column("GWTemp", TypeName = "decimal(18, 2)")]
        public decimal? Gwtemp { get; set; }
        [Column("SPVVolt", TypeName = "decimal(18, 2)")]
        public decimal? Spvvolt { get; set; }
        [Column("GWChargeS")]
        public int? GwchargeS { get; set; }
        [Column("GSM_SIG")]
        public int? GsmSig { get; set; }
        [Column("Connected_Cards")]
        public int? ConnectedCards { get; set; }
        [Column("RFSIG")]
        public int? Rfsig { get; set; }
        [Column("RFNWLEDState")]
        public int? Rfnwledstate { get; set; }
        [Column("C1ACTMin")]
        public int? C1actmin { get; set; }
        [Column("C2ACTMin")]
        public int? C2actmin { get; set; }
        [Column("C3ACTMin")]
        public int? C3actmin { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime AddedDateTime { get; set; }
        [Column("GWCtimeSoy")]
        public int? GwctimeSoy { get; set; }
        public int? GatewayId { get; set; }
    }
}
