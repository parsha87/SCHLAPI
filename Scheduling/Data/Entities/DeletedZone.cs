using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DeletedZone
    {
        [Key]
        [Column("DZId")]
        public int Dzid { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneNo { get; set; }
        public bool? Reused { get; set; }
        public int? ProjectId { get; set; }
    }
}
