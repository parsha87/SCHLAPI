using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("NodeLIveData")]
    public partial class NodeLiveData
    {
        [Key]
        public int Id { get; set; }
        public int? SysVerNo { get; set; }
        public int? BaseTime { get; set; }
        public int? DownLinkFreq { get; set; }
        public int? OudatedHsCount { get; set; }
        [Column("DownlinkSF")]
        public int? DownlinkSf { get; set; }
        [Column("DownLinkCR")]
        public int? DownLinkCr { get; set; }
        [Column("DownLinkPW")]
        public int? DownLinkPw { get; set; }
        public int? DownLinkSync { get; set; }
        public int? DownLinkPreamble { get; set; }
        public int? Window { get; set; }
        public int? JoinStatus { get; set; }
        public int? TempReading { get; set; }
        [Column("BAtteryVoltReading")]
        public int? BatteryVoltReading { get; set; }
        public int? LastSch { get; set; }
        public int? BlankScheduleRow { get; set; }
        public int? MoUpdateRxNo { get; set; }
        public int? ValveStatus { get; set; }
        [Column("DIStatus")]
        public int? Distatus { get; set; }
        [Column("TotalMO")]
        public int? TotalMo { get; set; }
        [Column("MOUpdateTxNo")]
        public int? MoupdateTxNo { get; set; }
        public int? UsNodeId { get; set; }
        public int? LastCommonTime { get; set; }
        public int? RxFrameType { get; set; }
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
        public int? Attempt3b { get; set; }
        public int? Power2b { get; set; }
        [Column("SFGW2b")]
        public int? Sfgw2b { get; set; }
        public int? GnSnrPrev3b { get; set; }
        public int? GnRssiPrev3b { get; set; }
        public int? ProductType4b { get; set; }
        public int? NodeId { get; set; }
    }
}
