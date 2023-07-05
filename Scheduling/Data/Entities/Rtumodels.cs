using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("RTUModels")]
    public partial class Rtumodels
    {
        [Key]
        [Column("RTUModelId")]
        public int RtumodelId { get; set; }
        [Column("RTUType")]
        [StringLength(200)]
        public string Rtutype { get; set; }
        public bool? Active { get; set; }
        public int? NoOfAnalogIp { get; set; }
        public int? NoOfDigitalIp { get; set; }
        public int? NoOfDigitalOp { get; set; }
        public bool? IsExpansionCardAllowed { get; set; }
        [Column("GlobalLIBId")]
        public int? GlobalLibid { get; set; }
    }
}
