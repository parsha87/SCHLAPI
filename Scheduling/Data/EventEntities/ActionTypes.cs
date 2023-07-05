using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class ActionTypes
    {
        public ActionTypes()
        {
            ManualOverrideMaster = new HashSet<ManualOverrideMaster>();
        }

        [Key]
        public int OperationCode { get; set; }
        [StringLength(50)]
        public string Action { get; set; }
        [StringLength(50)]
        public string ResetCondition { get; set; }
        public bool? IsActiveForRule { get; set; }

        [InverseProperty("ActionType")]
        public virtual ICollection<ManualOverrideMaster> ManualOverrideMaster { get; set; }
    }
}
