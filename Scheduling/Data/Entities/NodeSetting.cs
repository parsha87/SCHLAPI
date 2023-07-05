using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class NodeSetting
    {
        [Key]
        public int Id { get; set; }
        [Column("ThresholdOCSC")]
        public int? ThresholdOcsc { get; set; }
        public int? MinConnInterval { get; set; }
        public int? MaxConnInterval { get; set; }
        [Column("MInAdvInterval")]
        public int? MinAdvInterval { get; set; }
        public int? MaxAdvInterval { get; set; }
        [Column("MTUSize")]
        public int? Mtusize { get; set; }
        public int? HandshkInterval { get; set; }
        public int? PulseDelay { get; set; }
        public int? OperAttempt { get; set; }
        public int? AttemptForWaterFlow { get; set; }
        public int? LongSleepHndshkInterval { get; set; }
        [Column("BTTXPower")]
        public int? Bttxpower { get; set; }
        [Column("FixLoraSF")]
        public int? FixLoraSf { get; set; }
        public int? FixLoraPower { get; set; }
        public int? FixLoraFrequency { get; set; }
        [Column("FixLoraCR")]
        public int? FixLoraCr { get; set; }
        [Column("PreferredGW1Id")]
        public int? PreferredGw1id { get; set; }
        [Column("PreferredGW2Id")]
        public int? PreferredGw2id { get; set; }
        [Column("PreferredGW3Id")]
        public int? PreferredGw3id { get; set; }
        [Column("PreferredGW4Id")]
        public int? PreferredGw4id { get; set; }
        public int? AwfDetectEndis { get; set; }
        public int? FixLoraSetting { get; set; }
        public int? AutoSendStatus { get; set; }
        public int? PowerLoopLatchEnable { get; set; }
        [Column("SCHTransferedEnable")]
        public int? SchtransferedEnable { get; set; }
        public int? MaxLoraCommAtt { get; set; }
        [Column("SensorAlarmENdis")]
        public int? SensorAlarmEndis { get; set; }
        public int? LoraRxWindowMasking { get; set; }
        public int? DummyByte3Lsb { get; set; }
        [Column("DummyByte3MSB")]
        public int? DummyByte3Msb { get; set; }
        public int? SafetyTimeOut { get; set; }
        public int? NodeId { get; set; }
    }
}
