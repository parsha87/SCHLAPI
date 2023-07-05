using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class EventThresholdSetting
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("Event Type")]
        public string EventType { get; set; }
        public int? Good { get; set; }
        public int? Poor { get; set; }
        public int? Critical { get; set; }
    }
}
