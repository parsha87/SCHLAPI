using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class NotiActionList
    {
        public int? ActionListId { get; set; }
        public int? StepId { get; set; }

        [ForeignKey(nameof(ActionListId))]
        public virtual NotiElements ActionList { get; set; }
        [ForeignKey(nameof(StepId))]
        public virtual NotiSteps Step { get; set; }
    }
}
