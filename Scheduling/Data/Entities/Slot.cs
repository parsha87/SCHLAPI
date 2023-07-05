using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Slot
    {
        [Key]
        public int SlotId { get; set; }
        public int NetworkId { get; set; }
        [Column("RTUIdInNW")]
        public int? RtuidInNw { get; set; }
        public int ExpCardTypeId { get; set; }
        public bool? Active { get; set; }
        public int? PrjId { get; set; }
        [Column("RTUName")]
        [StringLength(100)]
        public string Rtuname { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public int? NoOfExpCards { get; set; }
        public int? BlockId { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? SamplingRate { get; set; }
        [Column("RTUId")]
        public int Rtuid { get; set; }
        [Column("ExpCardNoInRTU")]
        public int? ExpCardNoInRtu { get; set; }

        [ForeignKey(nameof(BlockId))]
        [InverseProperty("Slot")]
        public virtual Block Block { get; set; }
        [ForeignKey(nameof(ExpCardTypeId))]
        [InverseProperty(nameof(ExpansionCardType.Slot))]
        public virtual ExpansionCardType ExpCardType { get; set; }
        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(Project.Slot))]
        public virtual Project Prj { get; set; }
        [ForeignKey(nameof(Rtuid))]
        [InverseProperty("Slot")]
        public virtual Rtu Rtu { get; set; }
    }
}
