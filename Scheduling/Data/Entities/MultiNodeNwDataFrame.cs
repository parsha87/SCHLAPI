using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiNodeNwDataFrame
    {
        [Key]
        public int Id { get; set; }
        public int? TotalBytes { get; set; }
        public int? FrameType { get; set; }
        public int? NodeId { get; set; }
        public string LastCommTime { get; set; }
        public int? RxFrametype { get; set; }
        [Column("NGCurrentRSSI")]
        public int? NgcurrentRssi { get; set; }
        [Column("NGCurrentSNR")]
        public int? NgcurrentSnr { get; set; }
        [Column("NGCurrentSF")]
        public int? NgcurrentSf { get; set; }
        [Column("NGFreq")]
        public int? Ngfreq { get; set; }
        [Column("NGCurrentCR")]
        public int? NgcurrentCr { get; set; }
        [Column("GWId6b")]
        public int? Gwid6b { get; set; }
        [Column("NGAttempt3b")]
        public int? Ngattempt3b { get; set; }
        public int? Power2b { get; set; }
        [Column("SFGW2b")]
        public int? Sfgw2b { get; set; }
        [Column("GNSnrPrevious3b")]
        public int? GnsnrPrevious3b { get; set; }
        [Column("GNRssiPrevious3b")]
        public int? GnrssiPrevious3b { get; set; }
        public int? ProductType4b { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AddedDatetime { get; set; }
        public int? GwLastCommTime { get; set; }
        public int? NetworkNo { get; set; }
        public int? NodeNo { get; set; }
        public int? GatewayId { get; set; }
    }
}
