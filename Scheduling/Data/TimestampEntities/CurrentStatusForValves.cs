using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.TimestampEntities
{
    public partial class CurrentStatusForValves
    {
        [Key]
        public int Id { get; set; }
        [Column("RTUDateTime", TypeName = "datetime")]
        public DateTime? RtudateTime { get; set; }
        public int? PrjId { get; set; }
        public int? NetworkId { get; set; }
        [Column("RTUIdinNW")]
        public int? RtuidinNw { get; set; }
        public int? SlotId { get; set; }
        public int? ChannelId { get; set; }
        public int? ValveStatus { get; set; }
    }
}
