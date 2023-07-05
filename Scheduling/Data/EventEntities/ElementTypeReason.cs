using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class ElementTypeReason
    {
        [Key]
        public int EleTypeReasonId { get; set; }
        public int ElementTypeId { get; set; }
        [StringLength(15)]
        public string ElementType { get; set; }
        public int? ReasonId { get; set; }
        public string Reason { get; set; }
    }
}
