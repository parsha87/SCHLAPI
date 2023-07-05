using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class GroupDetails
    {
        public GroupDetails()
        {
            MasterPumpStationConfig = new HashSet<MasterPumpStationConfig>();
            ValveGroupConfig = new HashSet<ValveGroupConfig>();
        }

        [Key]
        public int GrpId { get; set; }
        public int NetworkId { get; set; }
        public int ZoneId { get; set; }
        public int OpGroupTypeId { get; set; }
        [StringLength(20)]
        public string GroupName { get; set; }
        [Column("GrpNoinNWZone")]
        public int? GrpNoinNwzone { get; set; }
        [StringLength(500)]
        public string TagName { get; set; }

        [InverseProperty("Grp")]
        public virtual ICollection<MasterPumpStationConfig> MasterPumpStationConfig { get; set; }
        [InverseProperty("Grp")]
        public virtual ICollection<ValveGroupConfig> ValveGroupConfig { get; set; }
    }
}
