using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DeletedSubBlock
    {
        [Key]
        [Column("DSId")]
        public int Dsid { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        public int? BlockId { get; set; }
        public int? SubBlockNo { get; set; }
        public bool? Reused { get; set; }
        public int? ProjectId { get; set; }
    }
}
