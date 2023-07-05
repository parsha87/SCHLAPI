using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiRtuAnalysis
    {
        [Key]
        public int Id { get; set; }
        public int? NetworkNo { get; set; }
        public int? NodeNo { get; set; }
        [StringLength(300)]
        public string EventType { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AddedDateTime { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? MinTemp { get; set; }
        public int? GatewayId { get; set; }
        [Column("NodeSF")]
        public int? NodeSf { get; set; }
        [Column("NodeSNR")]
        public int? NodeSnr { get; set; }
        public int? NodeRssi { get; set; }
        [Column("GatewaySF")]
        public int? GatewaySf { get; set; }
        [Column("GatewaySNR")]
        public int? GatewaySnr { get; set; }
        public int? GatewayRssi { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Battery { get; set; }
    }
}
