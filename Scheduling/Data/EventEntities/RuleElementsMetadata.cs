using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class RuleElementsMetadata
    {
        [Required]
        [StringLength(20)]
        public string ElementId { get; set; }
        [StringLength(50)]
        public string ElementName { get; set; }
        [StringLength(5)]
        public string ExeFrom { get; set; }
        [Column("IsActiveForMO")]
        public bool? IsActiveForMo { get; set; }
    }
}
