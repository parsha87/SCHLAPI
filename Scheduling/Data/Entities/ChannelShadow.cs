using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("Channel_Shadow")]
    public partial class ChannelShadow
    {
        [Key]
        public int Id { get; set; }
        public int ChannelId { get; set; }
        [Column("RTUId")]
        public int Rtuid { get; set; }
        public int EqpTypeId { get; set; }
        public int EqpId { get; set; }
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
        [Column("Shadow_SDT", TypeName = "datetime")]
        public DateTime ShadowSdt { get; set; }
        [Column("Shadow_EDT", TypeName = "datetime")]
        public DateTime ShadowEdt { get; set; }
        [Required]
        [StringLength(1)]
        public string ActionType { get; set; }
        [StringLength(128)]
        public string UserId { get; set; }
        [StringLength(25)]
        public string PhysicalLocation { get; set; }
    }
}
