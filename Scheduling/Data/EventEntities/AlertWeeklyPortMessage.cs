using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class AlertWeeklyPortMessage
    {
        [Key]
        public int WeeklyId { get; set; }
        public int PortNo { get; set; }
        [Required]
        [StringLength(50)]
        public string WeeklyFlag { get; set; }
    }
}
