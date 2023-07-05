using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class AlertReceiver
    {
        [Key]
        public int AlertReceiverId { get; set; }
        public int? AspNetUsersId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AlertSentDateTime { get; set; }
        public bool? IsReceived { get; set; }
    }
}
