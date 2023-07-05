using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("DeletedRTU")]
    public partial class DeletedRtu
    {
        [Key]
        [Column("DRId")]
        public int Drid { get; set; }
        public int? NetworkId { get; set; }
        [Column("RTUNo")]
        public int? Rtuno { get; set; }
        public bool? Reused { get; set; }
        public int? ProjectId { get; set; }
    }
}
