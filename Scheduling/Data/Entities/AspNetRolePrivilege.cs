using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class AspNetRolePrivilege
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Role { get; set; }
        public string Privilege { get; set; }
        [Column("RoleID")]
        [StringLength(128)]
        public string RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        [InverseProperty(nameof(AspNetRoles.AspNetRolePrivilege))]
        public virtual AspNetRoles RoleNavigation { get; set; }
    }
}
