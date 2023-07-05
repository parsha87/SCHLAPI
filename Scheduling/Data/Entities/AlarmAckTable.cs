using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class AlarmAckTable
    {
        [Key]
        public int Id { get; set; }
        public int? ReadAlarmId { get; set; }
        [StringLength(5)]
        public string Ack { get; set; }
        [StringLength(128)]
        public string UserId { get; set; }
    }
}
