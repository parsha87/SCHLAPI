using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiValveReason
    {
        [Key]
        public int Id { get; set; }
        [StringLength(500)]
        public string Reason { get; set; }
        public int? Value { get; set; }
        public int? Priorities { get; set; }
    }
}
