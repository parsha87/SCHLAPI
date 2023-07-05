using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class SubscriptionDetails
    {
        [Key]
        public int SubscriptionDetailId { get; set; }
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }
        public int? SubscriptionPeriod { get; set; }
        public int? SubscriptionUnitId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SubscriptionStartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SubscriptionEndDate { get; set; }
    }
}
