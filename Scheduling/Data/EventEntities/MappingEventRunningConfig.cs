using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class MappingEventRunningConfig
    {
        public int? EventId { get; set; }
        public int? RunningConfigId { get; set; }
    }
}
