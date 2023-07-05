using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class OutputGroupMembers
    {
        [Key]
        [Column("OPGroupTypeId")]
        public int OpgroupTypeId { get; set; }
        [Key]
        public int EqpTypeId { get; set; }
        [Key]
        public int ElementId { get; set; }
    }
}
