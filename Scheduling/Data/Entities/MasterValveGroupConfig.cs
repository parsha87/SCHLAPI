using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MasterValveGroupConfig
    {
        public MasterValveGroupConfig()
        {
            MasterValveGroupElementConfig = new HashSet<MasterValveGroupElementConfig>();
        }

        [Key]
        public int MstValveConfigId { get; set; }
        public int? ProjectId { get; set; }
        public int? NetworkId { get; set; }
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
        [Column("usMaster")]
        public int? UsMaster { get; set; }
        [Column("usMasterGroup")]
        public int? UsMasterGroup { get; set; }
        public int? GrpId { get; set; }
        [StringLength(25)]
        public string TagName { get; set; }

        [InverseProperty("MstValveConfig")]
        public virtual ICollection<MasterValveGroupElementConfig> MasterValveGroupElementConfig { get; set; }
    }
}
