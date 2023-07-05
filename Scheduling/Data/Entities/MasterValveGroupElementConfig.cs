using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MasterValveGroupElementConfig
    {
        [Key]
        [Column("MSTGrEleConfigId")]
        public int MstgrEleConfigId { get; set; }
        public int MstValveConfigId { get; set; }
        public int? ChannelId { get; set; }
        [Column("RTUId")]
        public int? Rtuid { get; set; }

        [ForeignKey(nameof(MstValveConfigId))]
        [InverseProperty(nameof(MasterValveGroupConfig.MasterValveGroupElementConfig))]
        public virtual MasterValveGroupConfig MstValveConfig { get; set; }
    }
}
