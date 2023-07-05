using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class QueueNetworkRelation
    {
        [Key]
        public int Id { get; set; }
        [StringLength(20)]
        public string QueueName { get; set; }
        public int? NetworkNo { get; set; }
    }
}
