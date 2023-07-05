using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class FertValveGroupSettingsConfig
    {
        [Key]
        [Column("FRTGrEleSettingsId")]
        public int FrtgrEleSettingsId { get; set; }
        [Column("MSTFertPumpId")]
        public int MstfertPumpId { get; set; }
        public int? ProjectId { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        public int? Waterbeforefertilizer { get; set; }
        public int? Typeofoperation { get; set; }
        public int? Duration { get; set; }
        public int? NominalSuctionRate { get; set; }
        [StringLength(10)]
        public string Unit { get; set; }
        public int FirtGrNo { get; set; }
        public int FirtGrSettingNo { get; set; }
    }
}
