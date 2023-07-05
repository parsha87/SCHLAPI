using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("Block_Shadow")]
    public partial class BlockShadow
    {
        [Key]
        public int Id { get; set; }
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
