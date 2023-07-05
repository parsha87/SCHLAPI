using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ValveGroupConfig
    {
        public ValveGroupConfig()
        {
            ValveGroupElementConfig = new HashSet<ValveGroupElementConfig>();
        }

        [Key]
        public int ValveConfigId { get; set; }
        public int? ProjectId { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        public int? OpGroupTypeId { get; set; }
        [StringLength(50)]
        public string GroupName { get; set; }
        public int? MaxPermGrFlow { get; set; }
        public int? GrpId { get; set; }
        [StringLength(25)]
        public string TagName { get; set; }

        [ForeignKey(nameof(GrpId))]
        [InverseProperty(nameof(GroupDetails.ValveGroupConfig))]
        public virtual GroupDetails Grp { get; set; }
        [InverseProperty("ValveConfig")]
        public virtual ICollection<ValveGroupElementConfig> ValveGroupElementConfig { get; set; }
    }
}
