using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class EquipmentConfigValues
    {
        [Key]
        [Column("RTUId")]
        public int Rtuid { get; set; }
        [Key]
        public int EqpId { get; set; }
        public int ConfigforEqpId { get; set; }
        [Key]
        [StringLength(100)]
        public string FieldName { get; set; }
        [StringLength(50)]
        public string FieldValue { get; set; }
        public int? PrjId { get; set; }

        [ForeignKey(nameof(Rtuid))]
        [InverseProperty("EquipmentConfigValues")]
        public virtual Rtu Rtu { get; set; }
    }
}
