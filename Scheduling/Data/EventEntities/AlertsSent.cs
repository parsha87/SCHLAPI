using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class AlertsSent
    {
        [Key]
        public int AlertsSentId { get; set; }
        public string AlertText { get; set; }
        [StringLength(128)]
        public string UserId { get; set; }
        [StringLength(10)]
        public string MobileNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SentDateTime { get; set; }
        public bool? IsSent { get; set; }
        public int? AlertTypeCondition { get; set; }
    }
}
