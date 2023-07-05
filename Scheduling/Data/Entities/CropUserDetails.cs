using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class CropUserDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string CropName { get; set; }
        [StringLength(100)]
        public string CropVariety { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? AreaUnderCrop { get; set; }
        [StringLength(100)]
        public string Unit { get; set; }
        [Column(TypeName = "date")]
        public DateTime? SowingDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? SeasonEndDate { get; set; }
        public int? GrowthStage { get; set; }
        [StringLength(100)]
        public string TypeofSystem { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? HeadSpacing { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? LateralSpacing { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? PressureatValveInlet { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DischargePerHead { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DischargePerSystem { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ApplicationRate { get; set; }
        public string UserId { get; set; }
        public int? SubblockId { get; set; }
        public int? CropId { get; set; }
    }
}
