using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class TimeIntervals
    {
        [Key]
        public int TimeSpanId { get; set; }
        [StringLength(50)]
        public string StartTime { get; set; }
        [StringLength(50)]
        public string EndTime { get; set; }
    }
}
