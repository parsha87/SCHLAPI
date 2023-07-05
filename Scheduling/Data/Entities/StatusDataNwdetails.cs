using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("StatusDataNWDetails")]
    public partial class StatusDataNwdetails
    {
        [Key]
        public int FrameId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime TimeStamp { get; set; }
        public int PrjId { get; set; }
        [Column("BSTId")]
        public int? Bstid { get; set; }
        [Column("BSTBatVolt")]
        [StringLength(10)]
        public string BstbatVolt { get; set; }
        [Column("BSTBatChargingStatus")]
        public bool? BstbatChargingStatus { get; set; }
        [Column("BSTGSMStrength")]
        [StringLength(10)]
        public string Bstgsmstrength { get; set; }
        [Column("CMDNo")]
        public int? Cmdno { get; set; }
        [Column("CMDLetters")]
        [StringLength(2)]
        public string Cmdletters { get; set; }
        [StringLength(1)]
        public string Status { get; set; }
        public int NetworkId { get; set; }
        [Column("MSGID")]
        [StringLength(200)]
        public string Msgid { get; set; }
    }
}
