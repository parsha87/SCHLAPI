using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class OperatorMetadata
    {
        [Key]
        public int OperatorId { get; set; }
        [StringLength(50)]
        public string Operator { get; set; }
    }
}
