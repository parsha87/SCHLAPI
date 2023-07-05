using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class CustomRulesConditions
    {
        [Key]
        public int ConditionId { get; set; }
        public int RuleId { get; set; }
        public int ConditionNo { get; set; }
        public int? ObjectId { get; set; }
        public int? EventObjTypeId { get; set; }
        public int? ElementValue { get; set; }
        public int? RangeValue1 { get; set; }
        public int? RangeValue2 { get; set; }
        public int? ConditionType { get; set; }
        public int? ConditionLink { get; set; }
        public string RuleQuery { get; set; }
        public string ShowQuery { get; set; }
        public string CompareFieldName { get; set; }
        public int? ElementType { get; set; }

        [ForeignKey(nameof(RuleId))]
        [InverseProperty(nameof(CustomRulesMaster.CustomRulesConditions))]
        public virtual CustomRulesMaster Rule { get; set; }
    }
}
