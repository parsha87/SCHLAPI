using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class NotificationUserAlert
    {
        [Key]
        public int NotiUserId { get; set; }
        [StringLength(50)]
        public string UserName { get; set; }
        [StringLength(10)]
        public string MobileNo { get; set; }
        [Column("BSTID")]
        public int? Bstid { get; set; }
    }
}
