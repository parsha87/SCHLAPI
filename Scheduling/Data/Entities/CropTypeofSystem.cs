using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class CropTypeofSystem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string TypeofSystem { get; set; }
    }
}
