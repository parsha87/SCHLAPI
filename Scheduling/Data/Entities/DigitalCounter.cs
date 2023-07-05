using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DigitalCounter
    {
        [Key]
        public int DcntId { get; set; }
        [StringLength(200)]
        public string Type { get; set; }
        public bool? Active { get; set; }
        public int? EqpTypeId { get; set; }
        [Column("GlobalLIBId")]
        public int? GlobalLibid { get; set; }

        [ForeignKey(nameof(EqpTypeId))]
        [InverseProperty(nameof(EquipmentType.DigitalCounter))]
        public virtual EquipmentType EqpType { get; set; }
    }
}
