using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Privilege
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [StringLength(100)]
        public string Action { get; set; }
        public int? ActionKey { get; set; }
        public bool? PrivilegeType { get; set; }
    }
}
