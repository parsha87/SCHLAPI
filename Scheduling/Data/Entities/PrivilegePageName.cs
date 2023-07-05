using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class PrivilegePageName
    {
        [Required]
        [Column("RoleID")]
        [StringLength(128)]
        public string RoleId { get; set; }
        [Required]
        public string PrevilegePages { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual AspNetRoles Role { get; set; }
    }
}
