using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class FertValveGroupElementsConfig
    {
        [Key]
        [Column("FRTGrEleConfigId")]
        public int FrtgrEleConfigId { get; set; }
        [Column("MSTFertPumpId")]
        public int MstfertPumpId { get; set; }
        public int? ProjectId { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        public int? ChannelId { get; set; }
        public int? TypeOfOp { get; set; }
        public int? Delay { get; set; }
        public int FirtGrNo { get; set; }
        public int FirtGrEleNo { get; set; }
    }
}
