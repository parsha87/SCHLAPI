using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiAlarmTypes
    {
        [Key]
        [Column("int")]
        public int Int { get; set; }
        [StringLength(500)]
        public string Description { get; set; }
        public int? Value { get; set; }
    }
}
