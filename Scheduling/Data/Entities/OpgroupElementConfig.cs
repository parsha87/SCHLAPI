using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("OPGroupElementConfig")]
    public partial class OpgroupElementConfig
    {
        [Key]
        [Column("OPGroupEleConfigId")]
        public int OpgroupEleConfigId { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        public int? OpgroupConfigId { get; set; }
        public int? ChannelId { get; set; }
        public int? NominalFlow { get; set; }
        [StringLength(50)]
        public string Unit { get; set; }

        [ForeignKey(nameof(OpgroupConfigId))]
        [InverseProperty("OpgroupElementConfig")]
        public virtual OpgroupConfig OpgroupConfig { get; set; }
    }
}
