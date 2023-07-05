using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class FilterValveGroupElementsConfig
    {
        [Key]
        [Column("FLTGrEleConfigId")]
        public int FltgrEleConfigId { get; set; }
        [Column("MSTfilterGroupId")]
        public int MstfilterGroupId { get; set; }
        public int? ChannelId { get; set; }
        public int? OpSeqOfFlush { get; set; }
        public int? PressSustainingOpNo { get; set; }

        [ForeignKey(nameof(MstfilterGroupId))]
        [InverseProperty(nameof(FilterValveGroupConfig.FilterValveGroupElementsConfig))]
        public virtual FilterValveGroupConfig MstfilterGroup { get; set; }
    }
}
