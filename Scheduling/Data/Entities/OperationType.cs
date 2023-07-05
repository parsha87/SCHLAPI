using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class OperationType
    {
        [Key]
        public int OperationTypeId { get; set; }
        [StringLength(50)]
        public string OperationName { get; set; }
    }
}
