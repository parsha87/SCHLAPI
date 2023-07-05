using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Program
    {
        [Key]
        public int ProgId { get; set; }
        [StringLength(20)]
        public string Name { get; set; }
    }
}
