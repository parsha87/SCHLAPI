using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiValveAlarmReason
    {
        [Key]
        public int Id { get; set; }
        public string Reason { get; set; }
        public int AlarmId { get; set; }
    }
}
