using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("RTUTemplate")]
    public partial class Rtutemplate
    {
        [Key]
        [Column("RTUTemplateId")]
        public int RtutemplateId { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [StringLength(25)]
        public string PhysicalLocation { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SamplingRate { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ScanningRate { get; set; }
        public bool IsProgrammable { get; set; }
        [Column("RFPowerLevel")]
        public int RfpowerLevel { get; set; }
        [Column("DCLatchPulseDuration")]
        public int DclatchPulseDuration { get; set; }
        public bool AuxSupply { get; set; }
        public int NoOfExpansioncard { get; set; }
        [Column("RTUModelId")]
        public int RtumodelId { get; set; }
        public bool? IsDefault { get; set; }
    }
}
