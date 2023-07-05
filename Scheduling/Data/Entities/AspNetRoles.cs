using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class AspNetRoles
    {
        public AspNetRoles()
        {
            AspNetRolePrivilege = new HashSet<AspNetRolePrivilege>();
            AspNetUserRoles = new HashSet<AspNetUserRoles>();
        }

        [Key]
        [StringLength(128)]
        public string Id { get; set; }
        [Required]
        [StringLength(256)]
        public string Name { get; set; }
        public bool? UserType { get; set; }
        public string ConcurrencyStamp { get; set; }
        [StringLength(265)]
        public string NormalizedName { get; set; }

        [InverseProperty("RoleNavigation")]
        public virtual ICollection<AspNetRolePrivilege> AspNetRolePrivilege { get; set; }
        [InverseProperty("Role")]
        public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; }
    }
}
