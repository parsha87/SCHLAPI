using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ValveStatus
    {
        public int? Id { get; set; }
        [Column("ValveStatus")]
        [StringLength(10)]
        public string ValveStatus1 { get; set; }
    }
}
