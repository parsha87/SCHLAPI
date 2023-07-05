using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class DeletedRules
    {
        [Key]
        [Column("DRid")]
        public int Drid { get; set; }
        public int RuleNo { get; set; }
        [Required]
        [StringLength(1)]
        public string ExeFrom { get; set; }
        public int NetworkId { get; set; }
        public bool Reused { get; set; }
    }
}
