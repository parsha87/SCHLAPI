using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class FilterValveGroupConfig
    {
        public FilterValveGroupConfig()
        {
            FilterValveGroupElementsConfig = new HashSet<FilterValveGroupElementsConfig>();
        }

        [Key]
        [Column("MSTfilterGroupId")]
        public int MstfilterGroupId { get; set; }
        public int? ProjectId { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        [Column("RTUId")]
        public int? Rtuid { get; set; }
        public int? OpGroupTypeId { get; set; }
        [StringLength(50)]
        public string GroupName { get; set; }
        public int? OperationType { get; set; }
        public bool? PauseWhileFlush { get; set; }
        [Column("MaxReiterationThroughPD")]
        public int? MaxReiterationThroughPd { get; set; }
        public int? ActionAfterMaxIteration { get; set; }
        public int? OffsetForFilterFlushinMin { get; set; }
        public int? FlushTimeinMin { get; set; }
        public int? DelayBetweenFlushinSec { get; set; }
        public int? WaterMeterNumber { get; set; }
        public int? RuleNoDirtSenseAlarm { get; set; }
        [Column("PDSensorNo")]
        public int? PdsensorNo { get; set; }
        public int? StartSustainingBeforeFlush { get; set; }
        public int? Timedeviation { get; set; }
        [Column("RuleNoWithPD")]
        public int? RuleNoWithPd { get; set; }
        public int? GrpId { get; set; }
        [StringLength(25)]
        public string TagName { get; set; }

        [InverseProperty("MstfilterGroup")]
        public virtual ICollection<FilterValveGroupElementsConfig> FilterValveGroupElementsConfig { get; set; }
    }
}
