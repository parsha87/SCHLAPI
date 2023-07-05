using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class UserRegisteration
    {
        [Key]
        [StringLength(128)]
        public string Id { get; set; }
        [StringLength(255)]
        public string FirstName { get; set; }
        [StringLength(255)]
        public string LastName { get; set; }
        [StringLength(255)]
        public string Address { get; set; }
        [StringLength(255)]
        public string Designation { get; set; }
        [StringLength(255)]
        public string WorkAreaLocation { get; set; }
        public int? RegisterAs { get; set; }
        [StringLength(100)]
        public string IfOther { get; set; }
        [StringLength(11)]
        public string MobNo { get; set; }
        [StringLength(256)]
        public string UserName { get; set; }
        [StringLength(256)]
        public string EmailAddress { get; set; }
        [StringLength(256)]
        public string UserId { get; set; }
        [StringLength(20)]
        public string Password { get; set; }
        [StringLength(20)]
        public string ConfirmPassword { get; set; }
        [StringLength(50)]
        public string PasswordHint { get; set; }
        public bool? IsUserEnabled { get; set; }
        [StringLength(50)]
        public string ProjectName { get; set; }
        public bool? IsConfigured { get; set; }
        [StringLength(128)]
        public string RoleId { get; set; }
    }
}
