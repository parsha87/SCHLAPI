using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("SubBlock_Shadow")]
    public partial class SubBlockShadow
    {
        [Key]
        public int Id { get; set; }
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
        [Column("Shadow_SDT", TypeName = "datetime")]
        public DateTime ShadowSdt { get; set; }
        [Column("Shadow_EDT", TypeName = "datetime")]
        public DateTime ShadowEdt { get; set; }
        [Required]
        [StringLength(1)]
        public string ActionType { get; set; }
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }
    }
}
