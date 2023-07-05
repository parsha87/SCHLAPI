using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MasterPumpStationConfig
    {
        [Key]
        [Column("MSTPumpStationId")]
        public int MstpumpStationId { get; set; }
        public int? ProjectId { get; set; }
        public int? NetworkId { get; set; }
        [Column("ZoneID")]
        public int? ZoneId { get; set; }
        [Column("RTUId")]
        public int? Rtuid { get; set; }
        public int? OpGroupTypeId { get; set; }
        [StringLength(50)]
        public string GroupName { get; set; }
        public int? StartDelay { get; set; }
        public int? StartDelayType { get; set; }
        public int? ActualDelay { get; set; }
        public int? CloseDelay { get; set; }
        public int? CloseDelayType { get; set; }
        public int? FailuareAction { get; set; }
        [Column("ControllingIPSensor")]
        public int? ControllingIpsensor { get; set; }
        [Column(TypeName = "decimal(18, 1)")]
        public decimal? StepUpThreshold { get; set; }
        [Column(TypeName = "decimal(18, 1)")]
        public decimal? StepDownThreshold { get; set; }
        [StringLength(50)]
        public string CriticalHighThreshold { get; set; }
        public int? Pumpstobeused { get; set; }
        public int? Stepstobeused { get; set; }
        public int? RuleNo { get; set; }
        public int? GrpId { get; set; }
        [StringLength(25)]
        public string TagName { get; set; }
        public int? AlaramMin { get; set; }
        public int? MaxUpDownAlarm { get; set; }
        public int? DelayToConfirmThreshold { get; set; }
        public int? SensorBasedOrNo { get; set; }
        public int? AlarmMin { get; set; }

        [ForeignKey(nameof(GrpId))]
        [InverseProperty(nameof(GroupDetails.MasterPumpStationConfig))]
        public virtual GroupDetails Grp { get; set; }
    }
}
