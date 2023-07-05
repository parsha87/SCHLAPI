using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("DigitalNONC")]
    public partial class DigitalNonc
    {
        [Key]
        public int DnoncId { get; set; }
        [StringLength(200)]
        public string Type { get; set; }
        public bool? Active { get; set; }
        public int? EqpTypeId { get; set; }
        [Column("GlobalLIBId")]
        public int? GlobalLibid { get; set; }

        [ForeignKey(nameof(EqpTypeId))]
        [InverseProperty(nameof(EquipmentType.DigitalNonc))]
        public virtual EquipmentType EqpType { get; set; }
    }
}
