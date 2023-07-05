using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class AlertConditions
    {
        [Key]
        public int AlertConditionId { get; set; }
        public int? TemplateId { get; set; }
        [StringLength(50)]
        public string Action { get; set; }
    }
}
