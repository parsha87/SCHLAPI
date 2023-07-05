using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiSensorAlarmReason
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public int? Value { get; set; }
    }
}
