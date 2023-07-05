using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class AdminRulesPrivileges
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }
        public int? Network { get; set; }
        public bool? AllNetwork { get; set; }
        public bool? Server { get; set; }
    }
}
