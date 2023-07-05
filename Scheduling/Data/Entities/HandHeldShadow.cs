using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("HandHeld_Shadow")]
    public partial class HandHeldShadow
    {
        [Key]
        [Column("HSId")]
        public int Hsid { get; set; }
        [Column("HHMasterId")]
        public int HhmasterId { get; set; }
        [Required]
        [StringLength(50)]
        public string HandHeldNo { get; set; }
        [Required]
        [StringLength(200)]
        public string FieldTechName { get; set; }
        [Column("Shadow_SDT", TypeName = "datetime")]
        public DateTime ShadowSdt { get; set; }
        [Column("Shadow_EDT", TypeName = "datetime")]
        public DateTime? ShadowEdt { get; set; }
        [Required]
        [StringLength(1)]
        public string ActionType { get; set; }
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
