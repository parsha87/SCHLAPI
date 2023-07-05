using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("StatusFrameNWDetails")]
    public partial class StatusFrameNwdetails
    {
        [Key]
        [Column(TypeName = "datetime")]
        public DateTime TimeStamp { get; set; }
        public int? PrjId { get; set; }
        [Column("BSTId")]
        public int? Bstid { get; set; }
        [Column("BSTBatVolt")]
        public int? BstbatVolt { get; set; }
        [Column("BSTBatChargingStatus")]
        public bool? BstbatChargingStatus { get; set; }
        [Column("BSTGSMStrength")]
        public int? Bstgsmstrength { get; set; }
        [Column("CMDNo")]
        public int? Cmdno { get; set; }
        [Column("CMDLetters")]
        [StringLength(2)]
        public string Cmdletters { get; set; }
        public int? NetworkId { get; set; }
    }
}
