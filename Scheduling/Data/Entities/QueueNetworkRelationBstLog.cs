using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class QueueNetworkRelationBstLog
    {
        [Key]
        public int Id { get; set; }
        [StringLength(300)]
        public string QueueName { get; set; }
        public bool? Status { get; set; }
    }
}
