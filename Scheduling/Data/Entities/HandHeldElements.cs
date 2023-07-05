using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class HandHeldElements
    {
        [Key]
        [Column("HHEleId")]
        public int HheleId { get; set; }
        [Column("HHMasterId")]
        public int HhmasterId { get; set; }
        public int NetworkId { get; set; }
    }
}
