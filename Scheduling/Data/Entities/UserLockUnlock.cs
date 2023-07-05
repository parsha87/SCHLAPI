using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class UserLockUnlock
    {
        [Key]
        public int LockUnlockId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [StringLength(1)]
        public string StatusForLock { get; set; }
    }
}
