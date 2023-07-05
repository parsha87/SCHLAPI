using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class SubBlock
    {
        public SubBlock()
        {
            SlotSubblockConnection = new HashSet<SlotSubblockConnection>();
        }

        [Key]
        public int SubblockId { get; set; }
        public int? BlockId { get; set; }
        public int? ZoneId { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public int? PrjId { get; set; }
        public int? ChannelId { get; set; }
        public int? SubBlockNo { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? MaxFlowRate { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? SubBlockArea { get; set; }
        [StringLength(25)]
        public string TagName { get; set; }
        [StringLength(50)]
        public string CropSystemType { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? HeadSpacing { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? LateralSpacing { get; set; }
        [Column("valveinletpressure", TypeName = "decimal(18, 3)")]
        public decimal? Valveinletpressure { get; set; }
        [Column("dischargeperhead", TypeName = "decimal(18, 3)")]
        public decimal? Dischargeperhead { get; set; }
        [Column("dischargepersystem", TypeName = "decimal(18, 3)")]
        public decimal? Dischargepersystem { get; set; }
        [Column("applicationrate", TypeName = "decimal(18, 3)")]
        public decimal? Applicationrate { get; set; }

        [ForeignKey(nameof(BlockId))]
        [InverseProperty("SubBlock")]
        public virtual Block Block { get; set; }
        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(Project.SubBlock))]
        public virtual Project Prj { get; set; }
        [InverseProperty("Subblock")]
        public virtual ICollection<SlotSubblockConnection> SlotSubblockConnection { get; set; }
    }
}
