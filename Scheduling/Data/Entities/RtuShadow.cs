using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("RTU_Shadow")]
    public partial class RtuShadow
    {
        [Key]
        public int Id { get; set; }
        [Column("RTUId")]
        public int Rtuid { get; set; }
        public int NetworkId { get; set; }
        [Column("RTUIdInNW")]
        public int? RtuidInNw { get; set; }
        [Column("RTUModelId")]
        public int RtumodelId { get; set; }
        public bool? Active { get; set; }
        public int? PrjId { get; set; }
        [Column("RTUName")]
        [StringLength(100)]
        public string Rtuname { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public int? NoOfExpCards { get; set; }
        public int? BlockId { get; set; }
        [StringLength(25)]
        public string PhysicalLocation { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? SamplingRate { get; set; }
        public bool? IsProgrammable { get; set; }
        [Column("RFPowerLevel")]
        public int? RfpowerLevel { get; set; }
        [Column("DCLatchPulseDuration")]
        public int? DclatchPulseDuration { get; set; }
        public bool? AuxSupply { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ScanningRate { get; set; }
        [Column("RTUNo")]
        public int? Rtuno { get; set; }
        [Column("IsSentToBST")]
        public bool? IsSentToBst { get; set; }
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
