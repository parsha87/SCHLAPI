using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DeletedSequence
    {
        [Key]
        [Column("DSqId")]
        public int DsqId { get; set; }
        public int? SeqNo { get; set; }
        public int? ProgramNo { get; set; }
        public bool? Reused { get; set; }
    }
}
