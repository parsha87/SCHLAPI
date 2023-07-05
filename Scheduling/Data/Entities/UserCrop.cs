using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class UserCrop
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }
        public int CropId { get; set; }
        public int CropTypeId { get; set; }
        [StringLength(200)]
        public string Variety { get; set; }
        [Column(TypeName = "decimal(18, 3)")]
        public decimal? AreaUnderCrop { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SowingDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? IrrigationSeasonEndDate { get; set; }
        public int? CropGrowthStageToday { get; set; }
        [StringLength(255)]
        public string TypeOfSystem { get; set; }
        public int? LateralSpacing { get; set; }
        public int? PressureAtTheValveInlet { get; set; }
        public int? DischargePerHead { get; set; }
        public int? DischargePerSystem { get; set; }
        public int? ApplicationRate { get; set; }
    }
}
