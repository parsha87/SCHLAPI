using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class SequenceDeleteStatus
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("isDeleting")]
        public bool? IsDeleting { get; set; }
    }
}
