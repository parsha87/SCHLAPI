using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DefaultUnits
    {
        [Key]
        public int Id { get; set; }
        public int EqpTypeId { get; set; }
        public int TypeId { get; set; }
        public int UnitId { get; set; }
        [StringLength(50)]
        public string UnitName { get; set; }
        public int? UnitTypeId { get; set; }
    }
}
