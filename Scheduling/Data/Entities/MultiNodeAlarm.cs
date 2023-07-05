using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiNodeAlarm
    {
        [Key]
        public int Id { get; set; }
        public int? FrameType { get; set; }
        public int? NodeId { get; set; }
        [Column("SOY")]
        public string Soy { get; set; }
        public int? AlarmType { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AddedDateTime { get; set; }
        public int? Gwsoy { get; set; }
        public int? NodeNo { get; set; }
        public int? NetworkNo { get; set; }
        public int? GatewayId { get; set; }
    }
}
