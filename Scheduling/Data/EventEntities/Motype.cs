using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    [Table("MOType")]
    public partial class Motype
    {
        [Key]
        [Column("MOTypeId")]
        public int MotypeId { get; set; }
        [Required]
        [StringLength(50)]
        public string Type { get; set; }
    }
}
