using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DashboardSensorsUserBased
    {
        [Key]
        public int Id { get; set; }
        public int? ChannelId { get; set; }
        [StringLength(128)]
        public string UserId { get; set; }
        public int? AssignedSensor { get; set; }
        public int? BlockId { get; set; }
        public int? SubBlockId { get; set; }
        public int? ZoneId { get; set; }
    }
}
