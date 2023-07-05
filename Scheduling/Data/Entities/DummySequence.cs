using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("dummySequence")]
    public partial class DummySequence
    {
        public DummySequence()
        {
            DummySeqMasterConfig = new HashSet<DummySeqMasterConfig>();
            DummySeqValveConfig = new HashSet<DummySeqValveConfig>();
            DummySeqWeeklySch = new HashSet<DummySeqWeeklySch>();
        }

        [Key]
        public int SeqId { get; set; }
        public int PrjId { get; set; }
        public int NetworkId { get; set; }
        public int ZoneId { get; set; }
        public bool? IsProgrammable { get; set; }
        [Column("BasisOfOP")]
        [StringLength(10)]
        public string BasisOfOp { get; set; }
        public int? IntervalDays { get; set; }
        public int? PrjTypeId { get; set; }
        public int? OperationTypeId { get; set; }
        [Column("MannualorAutoET")]
        [StringLength(1)]
        public string MannualorAutoEt { get; set; }
        [Column("IsSmartET")]
        public bool? IsSmartEt { get; set; }
        [StringLength(50)]
        public string SeqName { get; set; }
        [StringLength(200)]
        public string SeqDesc { get; set; }
        [StringLength(25)]
        public string SeqTagName { get; set; }
        [StringLength(3)]
        public string SeqType { get; set; }

        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(Project.DummySequence))]
        public virtual Project Prj { get; set; }
        [ForeignKey(nameof(PrjTypeId))]
        [InverseProperty(nameof(ProjectType.DummySequence))]
        public virtual ProjectType PrjType { get; set; }
        [InverseProperty("Seq")]
        public virtual ICollection<DummySeqMasterConfig> DummySeqMasterConfig { get; set; }
        [InverseProperty("Seq")]
        public virtual ICollection<DummySeqValveConfig> DummySeqValveConfig { get; set; }
        [InverseProperty("Seq")]
        public virtual ICollection<DummySeqWeeklySch> DummySeqWeeklySch { get; set; }
    }
}
