using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DeletedHandHeld
    {
        [Key]
        [Column("DHHId")]
        public int Dhhid { get; set; }
        [Required]
        [StringLength(200)]
        public string HandHeldNo { get; set; }
        public int Reused { get; set; }
        public int ProjectId { get; set; }
    }
}
