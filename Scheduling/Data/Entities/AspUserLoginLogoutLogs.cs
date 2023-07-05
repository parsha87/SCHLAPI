using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class AspUserLoginLogoutLogs
    {
        [Key]
        public int LoginUserId { get; set; }
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }
        [Required]
        [StringLength(256)]
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        [Column("RoleID")]
        [StringLength(256)]
        public string RoleId { get; set; }
        [StringLength(20)]
        public string MobileNo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UsrLoginTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UsrLogoutTime { get; set; }
        [StringLength(100)]
        public string LogoutReason { get; set; }
        public string ClientIp { get; set; }
        public string HostName { get; set; }
    }
}
