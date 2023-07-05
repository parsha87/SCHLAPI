using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("NRSeqUpids")]
    public partial class NrseqUpids
    {
        [Key]
        public int Id { get; set; }
        public int? NetworkNo { get; set; }
        public int? RtuNo { get; set; }
        public int? NodeRtuId { get; set; }
        public int? UpId { get; set; }
    }
}
