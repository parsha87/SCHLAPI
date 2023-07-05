using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("DigitalNO_NCTypeSensor")]
    public partial class DigitalNoNctypeSensor
    {
        [Key]
        public int Id { get; set; }
        public int? DfltStat { get; set; }
        public int? StatRevDly { get; set; }
        public int? AlrmLvl { get; set; }
        public int? DlyTmeIfRevr { get; set; }
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
    }
}
