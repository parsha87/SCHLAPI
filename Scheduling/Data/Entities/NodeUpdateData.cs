using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class NodeUpdateData
    {
        [Key]
        public int Id { get; set; }
        public int? ConfigSettingUpdateNo { get; set; }
        public int? ScheduleUpdateNo { get; set; }
        public int? InternalMoUpdateNo { get; set; }
        public int? ExtraByte { get; set; }
        public int? NodeId { get; set; }
    }
}
