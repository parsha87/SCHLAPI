using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Channel
    {
        [Key]
        public int ChannelId { get; set; }
        [Key]
        [Column("RTUId")]
        public int Rtuid { get; set; }
        [Key]
        public int EqpTypeId { get; set; }
        [Key]
        public int EqpId { get; set; }
        [Key]
        [Column("SlotIdInRTU")]
        public int SlotIdInRtu { get; set; }
        public int TypeId { get; set; }
        [Column("SubTypeID")]
        public int? SubTypeId { get; set; }
        public bool? IsExpansionCardSlot { get; set; }
        public int? PrjId { get; set; }
        public int? SlotId { get; set; }
        [StringLength(50)]
        public string ChannelName { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public bool? IsEnabled { get; set; }
        public bool? IsActive { get; set; }
        public bool? UsedInSubBlock { get; set; }
        [StringLength(20)]
        public string TagName { get; set; }
        public bool? UsedInGrp { get; set; }
        [StringLength(25)]
        public string PhysicalLocation { get; set; }
        [Column("defaltflag")]
        public int? Defaltflag { get; set; }

        [ForeignKey(nameof(EqpTypeId))]
        [InverseProperty(nameof(EquipmentType.Channel))]
        public virtual EquipmentType EqpType { get; set; }
        [ForeignKey(nameof(Rtuid))]
        [InverseProperty("Channel")]
        public virtual Rtu Rtu { get; set; }
    }
}
