using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class ThresholdCrossingAlarmsMaster
    {
        [Key]
        public int ThresholdAlarmId { get; set; }
        public int? NetworkId { get; set; }
        [Column("BSTId")]
        public int? Bstid { get; set; }
        [Column("BSTBatVolt")]
        public int? BstbatVolt { get; set; }
        [Column("BSTBatCharging")]
        public bool? BstbatCharging { get; set; }
        [Column("BSTGSMStrength")]
        public int? Bstgsmstrength { get; set; }
    }
}
