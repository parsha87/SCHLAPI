using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class SubscriptionUnit
    {
        [Key]
        public int SubscriptionUnitId { get; set; }
        [Required]
        [StringLength(200)]
        public string SubscriptionUnitType { get; set; }
    }
}
