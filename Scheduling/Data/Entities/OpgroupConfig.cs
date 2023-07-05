using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("OPGroupConfig")]
    public partial class OpgroupConfig
    {
        public OpgroupConfig()
        {
            OpgroupElementConfig = new HashSet<OpgroupElementConfig>();
        }

        [Key]
        public int OpGroupConfigId { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        [StringLength(50)]
        public string GroupName { get; set; }
        [Column("MSTGroupName")]
        [StringLength(50)]
        public string MstgroupName { get; set; }
        [Column("MSTFertPumpId")]
        public int? MstfertPumpId { get; set; }
        [Column("MSTFilterGroupId")]
        public int? MstfilterGroupId { get; set; }
        public int? OpGroupTypeId { get; set; }

        [InverseProperty("OpgroupConfig")]
        public virtual ICollection<OpgroupElementConfig> OpgroupElementConfig { get; set; }
    }
}
