using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DeletedBlock
    {
        [Key]
        [Column("DBId")]
        public int Dbid { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        public int? BlockNo { get; set; }
        public bool? Reused { get; set; }
        public int? ProjectId { get; set; }
    }
}
