using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class UpdateIds
    {
        [Key]
        public int Id { get; set; }
        [Column("GWId")]
        public int? Gwid { get; set; }
        public int? NodeId { get; set; }
        public int? NodeUid { get; set; }
        public int? ConfigUid { get; set; }
        public int? VrtUid { get; set; }
        public int? SensorUid { get; set; }
        public int? ScheduleNodeUid { get; set; }
        public int? ScheduleSequenceUid { get; set; }
        public int? MainSchUid { get; set; }
        public int? FilterUid { get; set; }
    }
}
