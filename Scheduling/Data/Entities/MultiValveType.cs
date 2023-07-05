using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiValveType
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string ValveType { get; set; }
    }
}
