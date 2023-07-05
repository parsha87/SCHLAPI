using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiNodeJoinDataFrame
    {
        [Key]
        public int Id { get; set; }
        public int? TotalBytes { get; set; }
        public int? FrameType { get; set; }
        public int? NodeId { get; set; }
        public string LastCommTime { get; set; }
        public int? DeviceResetCause { get; set; }
        public int? Attempt { get; set; }
        public int? ProjectId { get; set; }
        public int? TechId { get; set; }
        public int? NodeNo { get; set; }
        [Column("GWno1")]
        public int? Gwno1 { get; set; }
        public int? Gwno2 { get; set; }
        public int? Gwno3 { get; set; }
        public int? Gwno4 { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Longitude { get; set; }
        [StringLength(500)]
        public string Moy { get; set; }
        public int? Seconds { get; set; }
        public int? Time { get; set; }
        public int? Year { get; set; }
        public long? LatLongAccuracy { get; set; }
        [Column("DeviceSRNo")]
        public long? DeviceSrno { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AddedDateTime { get; set; }
        [Column("GWMoy")]
        public int? Gwmoy { get; set; }
        public int? GwLastCommTime { get; set; }
        public int? GatewayId { get; set; }
        public int? NetworkNo { get; set; }
    }
}
