using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class NonRechableNode
    {
        [Key]
        public int Id { get; set; }
        [Column("ThresholdforOCSC")]
        public int? ThresholdforOcsc { get; set; }
        public int? MinConnInterval { get; set; }
        public int? MaxConnInterval { get; set; }
        public int? MinAdvInterval { get; set; }
        public int? MaxAdvInterval { get; set; }
        [Column("MTUSizeValue")]
        public int? MtusizeValue { get; set; }
        public int? HandshkInterval { get; set; }
        public int? Pulsedelayvalue { get; set; }
        public int? OperAttempt { get; set; }
        public int? AttemptForWaterFlow { get; set; }
        [Column("LongSleepHndshkIntervalMF")]
        public int? LongSleepHndshkIntervalMf { get; set; }
        [Column("BTTXPower")]
        public int? Bttxpower { get; set; }
        [Column("FixLoraSF")]
        public int? FixLoraSf { get; set; }
        public int? FixLoraPower { get; set; }
        public int? FixLoraFreq { get; set; }
        [Column("FixLoraCR")]
        public int? FixLoraCr { get; set; }
        [Column("PreferredGW1ID")]
        public int? PreferredGw1id { get; set; }
        [Column("PreferredGW2ID")]
        public int? PreferredGw2id { get; set; }
        [Column("PreferredGW3ID")]
        public int? PreferredGw3id { get; set; }
        [Column("PreferredGW4ID")]
        public int? PreferredGw4id { get; set; }
        [Column("AWFDetectENDIS")]
        public int? AwfdetectEndis { get; set; }
        public int? FixLoraSetting { get; set; }
        public int? AutoSendStatusEnableBit { get; set; }
        public int? PowerLoopLatchEnable { get; set; }
        public int? GlobalAlarmEnDs { get; set; }
        public int? MaxLoRaCommAtt { get; set; }
        [Column("SensorAlarmENDIS")]
        public int? SensorAlarmEndis { get; set; }
        public int? LoRaRxWindowMasking { get; set; }
        [Column("DummyByte3LSB")]
        public int? DummyByte3Lsb { get; set; }
        [Column("DummyByte3MSB")]
        public int? DummyByte3Msb { get; set; }
        public int? SaftetyTimeoutMin { get; set; }
        public int? SlowDownCommDurMin { get; set; }
        public int? ForceDeepSleepDurMin { get; set; }
        public int? NodeId { get; set; }
        public int? GwSrn { get; set; }
        public int? ProductId { get; set; }
        public string TagName { get; set; }
        public string NetworkTagName { get; set; }
        public string NodeTagName { get; set; }
    }
}
