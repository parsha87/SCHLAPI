using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class SensorUnitTypeMapping
    {
        [Key]
        public int Id { get; set; }
        public int UnitTypeId { get; set; }
        public int EqpTypeId { get; set; }
        public int TypeId { get; set; }
        public bool Active { get; set; }
        [Column("GlobalLIBId")]
        public int? GlobalLibid { get; set; }
    }
}
