using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class UpdateStatisticLog
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime JobExecuteDate { get; set; }
        public bool IsExecuted { get; set; }
        [Required]
        [StringLength(255)]
        public string DbName { get; set; }
    }
}
