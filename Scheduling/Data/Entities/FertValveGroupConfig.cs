using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class FertValveGroupConfig
    {
        [Key]
        [Column("MSTFertPumpId")]
        public int MstfertPumpId { get; set; }
        public int? ProjectId { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        [Column("RTUId")]
        public int? Rtuid { get; set; }
        public int? OpGroupTypeId { get; set; }
        [StringLength(50)]
        public string GroupName { get; set; }
        public int? FertCounterNo { get; set; }
        public int? NoCommunicationTime { get; set; }
        public bool? IfNoCommunication { get; set; }
        public int FirtGrNo { get; set; }
        public int? GrpId { get; set; }
        [StringLength(1000)]
        public string Tag { get; set; }
        public int? WaterBeforeFert { get; set; }
        [StringLength(25)]
        public string TagName { get; set; }
    }
}
