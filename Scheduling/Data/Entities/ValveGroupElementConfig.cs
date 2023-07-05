using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ValveGroupElementConfig
    {
        [Key]
        [Column("VLVGrEleConfigId")]
        public int VlvgrEleConfigId { get; set; }
        public int ValveConfigId { get; set; }
        public int? ChannelId { get; set; }

        [ForeignKey(nameof(ValveConfigId))]
        [InverseProperty(nameof(ValveGroupConfig.ValveGroupElementConfig))]
        public virtual ValveGroupConfig ValveConfig { get; set; }
    }
}
