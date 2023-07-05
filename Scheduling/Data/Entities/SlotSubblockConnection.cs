using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class SlotSubblockConnection
    {
        [Key]
        public int PrjId { get; set; }
        [Key]
        [Column("RTUId")]
        public int Rtuid { get; set; }
        [Key]
        public int SlotSeqId { get; set; }
        [Key]
        public int BlockId { get; set; }
        public int? SubblockId { get; set; }

        [ForeignKey(nameof(BlockId))]
        [InverseProperty("SlotSubblockConnection")]
        public virtual Block Block { get; set; }
        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(Project.SlotSubblockConnection))]
        public virtual Project Prj { get; set; }
        [ForeignKey(nameof(Rtuid))]
        [InverseProperty("SlotSubblockConnection")]
        public virtual Rtu Rtu { get; set; }
        [ForeignKey(nameof(SubblockId))]
        [InverseProperty(nameof(SubBlock.SlotSubblockConnection))]
        public virtual SubBlock Subblock { get; set; }
    }
}
