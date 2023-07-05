using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class EventsProcessingStatus
    {
        [Key]
        public int ProcessId { get; set; }
        public int? NetworkNo { get; set; }
        [Column("status")]
        public int? Status { get; set; }
    }
}
