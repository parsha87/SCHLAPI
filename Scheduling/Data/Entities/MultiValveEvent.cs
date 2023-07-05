using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiValveEvent
    {
        [Key]
        public int Id { get; set; }
        public int? TotalBytes { get; set; }
        public int? FrameType { get; set; }
        public int? NodeId { get; set; }
        [StringLength(500)]
        public string UpdataeTimeSow { get; set; }
        public int? ValveNo { get; set; }
        public int? ValveType { get; set; }
        public int? RequiredState { get; set; }
        public int? CurrentState { get; set; }
        public int? CurrentStateReason { get; set; }
        [Column("OperationTImeMOY")]
        [StringLength(500)]
        public string OperationTimeMoy { get; set; }
        public int? ActiveCurrent { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AddedDateTime { get; set; }
        [Column("GWOperationTimeMoy")]
        public int? GwoperationTimeMoy { get; set; }
        public int? NodeNo { get; set; }
        public int? NetworkNo { get; set; }
        public int? GatewayId { get; set; }
    }
}
