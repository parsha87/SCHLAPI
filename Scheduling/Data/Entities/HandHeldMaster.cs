using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class HandHeldMaster
    {
        [Key]
        [Column("HHMasterId")]
        public int HhmasterId { get; set; }
        [Required]
        [StringLength(50)]
        public string HandHeldNo { get; set; }
        [Required]
        [StringLength(200)]
        public string FieldTechName { get; set; }
        [Required]
        public string Description { get; set; }
        [Column("ProjectID")]
        public int ProjectId { get; set; }
    }
}
