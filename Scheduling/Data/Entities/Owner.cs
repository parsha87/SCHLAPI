using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Owner
    {
        [Key]
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public int? ZoneId { get; set; }
        public int? BlockId { get; set; }
        public int? SubBlockId { get; set; }
        [StringLength(128)]
        public string UserId { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? OwnerSubBlockArea { get; set; }
    }
}
