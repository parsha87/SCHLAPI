using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class CustomRulesMaster
    {
        public CustomRulesMaster()
        {
            CustomRulesConditions = new HashSet<CustomRulesConditions>();
            CustomRulesTarget = new HashSet<CustomRulesTarget>();
        }

        [Key]
        public int RuleId { get; set; }
        public int RuleNo { get; set; }
        [StringLength(50)]
        public string RuleName { get; set; }
        [StringLength(10)]
        public string StartTime { get; set; }
        [StringLength(10)]
        public string EndTime { get; set; }
        [StringLength(50)]
        public string Description { get; set; }
        [StringLength(1)]
        public string RuleExeFrom { get; set; }
        public int? NetwrokNo { get; set; }
        public int? AlarmLevel { get; set; }
        public int? Action { get; set; }
        public bool? IsDeleted { get; set; }
        public string ShowQuery { get; set; }
        public string RuleQuery { get; set; }
        [StringLength(10)]
        public string DelayToConfirm { get; set; }
        [StringLength(10)]
        public string RuleResetTime { get; set; }
        public int? RuleStatus { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RuleExecutedOn { get; set; }
        public bool? IsManualReset { get; set; }
        public string UserName { get; set; }
        [StringLength(25)]
        public string TagName { get; set; }
        [StringLength(10)]
        public string RuleRepeatTime { get; set; }
        [StringLength(10)]
        public string RuleRearmTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Rulenotifiedon { get; set; }
        [StringLength(10)]
        public string OffsetTime { get; set; }

        [InverseProperty("Rule")]
        public virtual ICollection<CustomRulesConditions> CustomRulesConditions { get; set; }
        [InverseProperty("Rule")]
        public virtual ICollection<CustomRulesTarget> CustomRulesTarget { get; set; }
    }
}
