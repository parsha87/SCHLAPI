using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Accessor
    {
        [Key]
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public int? ZoneId { get; set; }
        public int? BlockId { get; set; }
        public int? SubBlockId { get; set; }
        [StringLength(128)]
        public string UserId { get; set; }
        public int? TestForCompare { get; set; }
    }
}
