using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("Analog0_5VSensor")]
    public partial class Analog05vsensor
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? CtrHghThrs { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? HghThrs { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? LwTthrs { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? CrtLwrThrs { get; set; }
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
        [StringLength(500)]
        public string SensorName { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ScaleMin { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ScaleMax { get; set; }
    }
}
