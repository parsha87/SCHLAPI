using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class RunningEventsConfig
    {
        [Key]
        public int Id { get; set; }
        public int? PrjId { get; set; }
        public int? NetworkId { get; set; }
        public int? ObjTypeId { get; set; }
        public int? NotiStepId { get; set; }
        [StringLength(15)]
        public string Status { get; set; }
        public int? Remaining5kResponseId { get; set; }
    }
}
