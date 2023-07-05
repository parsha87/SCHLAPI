using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class AspNetUsers
    {
        public AspNetUsers()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaims>();
            AspNetUserLogins = new HashSet<AspNetUserLogins>();
            AspNetUserRoles = new HashSet<AspNetUserRoles>();
        }

        [Key]
        [StringLength(128)]
        public string Id { get; set; }
        [StringLength(256)]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        [Required]
        [StringLength(256)]
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Column("EmailID")]
        public string EmailId { get; set; }
        public bool? UsedInPrj { get; set; }
        public int? PrjId { get; set; }
        [StringLength(256)]
        public string Address { get; set; }
        [Column("RoleID")]
        [StringLength(256)]
        public string RoleId { get; set; }
        [StringLength(20)]
        public string MobileNo { get; set; }
        [StringLength(255)]
        public string Designation { get; set; }
        [StringLength(255)]
        public string WorkAreaLocation { get; set; }
        public int? RegisterAs { get; set; }
        [StringLength(100)]
        public string IfOther { get; set; }
        [StringLength(50)]
        public string PasswordHint { get; set; }
        public bool? IsUserEnabled { get; set; }
        public bool? IsConfigured { get; set; }
        [StringLength(255)]
        public string EncreptedPassword { get; set; }
        public bool? IsRestrictedUser { get; set; }
        public int? UserNo { get; set; }
        public int? CountryId { get; set; }
        public int? LanguageId { get; set; }
        [StringLength(265)]
        public string NormalizedEmail { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public string ConcurrencyStamp { get; set; }
        [StringLength(265)]
        public string NormalizedUserName { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; }
    }
}
