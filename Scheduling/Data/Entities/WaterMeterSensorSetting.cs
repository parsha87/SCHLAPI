using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class WaterMeterSensorSetting
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? HghFlwRtFrq { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? LowFlwRtFrq { get; set; }
        public int? TmeCfrmNoFlwSec { get; set; }
        public int? DlyCfmThrs { get; set; }
        public int? Rsved { get; set; }
        public int? NodeId { get; set; }
        public int? SsNo { get; set; }
        public int? SsType { get; set; }
        [Column("SSPriority")]
        public int? Sspriority { get; set; }
        public int? SamplingRate { get; set; }
        public int? ProductType { get; set; }
        public int? GwSrn { get; set; }
        public int? NodePorductId { get; set; }
        public string TagName { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? PulseValue { get; set; }
    }
}
