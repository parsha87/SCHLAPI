using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Block
    {
        public Block()
        {
            Rtu = new HashSet<Rtu>();
            Slot = new HashSet<Slot>();
            SlotSubblockConnection = new HashSet<SlotSubblockConnection>();
            SubBlock = new HashSet<SubBlock>();
        }

        [Key]
        public int BlockId { get; set; }
        public int? PrjId { get; set; }
        public int? ZoneId { get; set; }
        public int? NetworkId { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public int? NoOfSubBlock { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? MaxFlowRate { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public int? BlockNo { get; set; }
        public int? BlockNoForFrame { get; set; }
        [StringLength(25)]
        public string TagName { get; set; }

        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(Project.Block))]
        public virtual Project Prj { get; set; }
        [InverseProperty("Block")]
        public virtual ICollection<Rtu> Rtu { get; set; }
        [InverseProperty("Block")]
        public virtual ICollection<Slot> Slot { get; set; }
        [InverseProperty("Block")]
        public virtual ICollection<SlotSubblockConnection> SlotSubblockConnection { get; set; }
        [InverseProperty("Block")]
        public virtual ICollection<SubBlock> SubBlock { get; set; }
    }
}
