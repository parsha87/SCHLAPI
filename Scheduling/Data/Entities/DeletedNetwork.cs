using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DeletedNetwork
    {
        [Key]
        [Column("DNId")]
        public int Dnid { get; set; }
        public int? NetworkId { get; set; }
        public int? NetworkNo { get; set; }
        public bool? Reused { get; set; }
        public int? ProjectId { get; set; }
    }
}
