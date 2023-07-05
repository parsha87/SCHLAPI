using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class SequenceMasterConfig
    {
        public SequenceMasterConfig()
        {
            SequenceValveConfig = new HashSet<SequenceValveConfig>();
        }

        [Key]
        [Column("MSTSeqId")]
        public int MstseqId { get; set; }
        public int? SeqId { get; set; }
        public int? ProjectId { get; set; }
        public int? PrgId { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        public int? StartId { get; set; }
        [StringLength(10)]
        public string StartTime { get; set; }

        [ForeignKey(nameof(SeqId))]
        [InverseProperty(nameof(Sequence.SequenceMasterConfig))]
        public virtual Sequence Seq { get; set; }
        [InverseProperty("Mstseq")]
        public virtual ICollection<SequenceValveConfig> SequenceValveConfig { get; set; }
    }
}
