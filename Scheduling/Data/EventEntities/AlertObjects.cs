using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class AlertObjects
    {
        [Key]
        public int AlertObjectId { get; set; }
        [StringLength(1000)]
        public string ObjectName { get; set; }
    }
}
