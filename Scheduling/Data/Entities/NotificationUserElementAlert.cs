using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class NotificationUserElementAlert
    {
        [Key]
        public int NotiElId { get; set; }
        public int? NotiUserId { get; set; }
        public int? NetworkId { get; set; }
    }
}
