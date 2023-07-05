using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class RuleExecutionHistory
    {
        [Key]
        public int RuleExeId { get; set; }
        public int RuleId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime RuleExecutedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime RuleOffsetDate { get; set; }
        public int Status { get; set; }
        public int ParentId { get; set; }
        [Required]
        public string UserName { get; set; }
        public int Active { get; set; }
    }
}
