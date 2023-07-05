using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class CustomRulesTarget
    {
        [Key]
        public int TargetId { get; set; }
        public int? RuleId { get; set; }
        public int? ObjectId { get; set; }
        public int? EventObjTypeId { get; set; }

        [ForeignKey(nameof(RuleId))]
        [InverseProperty(nameof(CustomRulesMaster.CustomRulesTarget))]
        public virtual CustomRulesMaster Rule { get; set; }
    }
}
