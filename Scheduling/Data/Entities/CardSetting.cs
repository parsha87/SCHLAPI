using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class CardSetting
    {
        [Key]
        public int Id { get; set; }
        public int? CardNo { get; set; }
        public int? CardType { get; set; }
        public int? UcDebug { get; set; }
        public int? UcWarning { get; set; }
        public int? UcError { get; set; }
        public int? UcInfo { get; set; }
        public int? SleepEnDis { get; set; }
        public int? AutoSendStatusEnDis { get; set; }
        public int? PrediocInvervalSec { get; set; }
        public int? LogIntervalSec { get; set; }
        public int? CardFuVersionNo { get; set; }
        public int? SafetyTimeoutValves { get; set; }
        public int? Reserved { get; set; }
        [Column("FNominalOperatioCurrent")]
        public int? FnominalOperatioCurrent { get; set; }
        public int? PulseDurationMs { get; set; }
        public int? PulseVoltage { get; set; }
        public int? TypeSeleniodLatch { get; set; }
        [Column("SMTS1Address")]
        public int? Smts1address { get; set; }
        [Column("SMTS1Level")]
        public int? Smts1level { get; set; }
        [Column("SMTS2Address")]
        public int? Smts2address { get; set; }
        [Column("SMTS2Level")]
        public int? Smts2level { get; set; }
        public int? ProductId { get; set; }
        public int? NodeId { get; set; }
    }
}
