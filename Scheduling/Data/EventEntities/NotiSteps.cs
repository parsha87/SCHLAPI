using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class NotiSteps
    {
        [Key]
        public int StepId { get; set; }
        [StringLength(100)]
        public string Step { get; set; }
    }
}
