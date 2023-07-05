using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class ReadyAlarms
    {
        public int PrjId { get; set; }
        public int NetworkId { get; set; }
        [Key]
        public int AlarmId { get; set; }
        public int RuleId { get; set; }
        public int AlarmLevel { get; set; }
        public int EqpTypeId { get; set; }
        [Column("ObjectIdInDB")]
        public int ObjectIdInDb { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AddedDateTime { get; set; }
        public int? Priority { get; set; }
        public int? CurrentValue { get; set; }
        public bool? IsProcessed { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ProcessedDateTime { get; set; }
    }
}
