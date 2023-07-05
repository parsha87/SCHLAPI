using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Equipment
    {
        [Key]
        public int EqpId { get; set; }
        public int? EqpTypeId { get; set; }
        public int? TypeId { get; set; }
        [Column("SubTypeID")]
        public int? SubTypeId { get; set; }
        public bool? Active { get; set; }
        public int PrjId { get; set; }

        [ForeignKey(nameof(EqpTypeId))]
        [InverseProperty(nameof(EquipmentType.Equipment))]
        public virtual EquipmentType EqpType { get; set; }
        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(Project.Equipment))]
        public virtual Project Prj { get; set; }
    }
}
