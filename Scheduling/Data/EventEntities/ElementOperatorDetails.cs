using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class ElementOperatorDetails
    {
        [Key]
        public int TypeId { get; set; }
        [StringLength(50)]
        public string ElementType { get; set; }
        public int OperatorId { get; set; }
    }
}
