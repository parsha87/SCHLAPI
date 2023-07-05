using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class IrrigationMultiFactor
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal MultiplyingConstant { get; set; }
    }
}
