using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("dummySeqMasterConfig")]
    public partial class DummySeqMasterConfig
    {
        [Key]
        [Column("MSTSeqId")]
        public int MstseqId { get; set; }
        public int? SeqId { get; set; }
        public int? StartId { get; set; }
        [StringLength(10)]
        public string StartTime { get; set; }

        [ForeignKey(nameof(SeqId))]
        [InverseProperty(nameof(DummySequence.DummySeqMasterConfig))]
        public virtual DummySequence Seq { get; set; }
    }
}
