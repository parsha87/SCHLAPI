using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiValveState
    {
        [Key]
        public int Id { get; set; }
        [StringLength(200)]
        public string State { get; set; }
        public int? Value { get; set; }
    }
}
