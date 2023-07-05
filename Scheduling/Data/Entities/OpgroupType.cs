using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("OPGroupType")]
    public partial class OpgroupType
    {
        [Key]
        public int OpGroupTypeId { get; set; }
        [StringLength(200)]
        public string Type { get; set; }
        public bool? Active { get; set; }
        public bool? IsProgrammable { get; set; }
    }
}
