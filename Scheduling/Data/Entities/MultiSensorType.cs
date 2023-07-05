using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiSensorType
    {
        [Key]
        public int Id { get; set; }
        public string AorD { get; set; }
        public string SensorDescription { get; set; }
        [Column("SSType")]
        public int? Sstype { get; set; }
    }
}
