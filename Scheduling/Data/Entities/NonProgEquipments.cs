using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class NonProgEquipments
    {
        [Key]
        public int NonPrgEqpId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        public int? PrjId { get; set; }

        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(Project.NonProgEquipments))]
        public virtual Project Prj { get; set; }
    }
}
