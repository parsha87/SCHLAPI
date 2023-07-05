using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class WaterMeterCumulative
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        public int? NetworkId { get; set; }
        [Column("RTUIdInNw")]
        public int? RtuidInNw { get; set; }
        public int? SlotIdInNw { get; set; }
        public int? DigitalCounterVal { get; set; }
        public int? TotalCumulative { get; set; }
        public int? SensorValue { get; set; }
        public int? CumulativeCount { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ResetDate { get; set; }
    }
}
