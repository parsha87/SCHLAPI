using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class GatewayNode
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Operator1MobNo { get; set; }
        [StringLength(50)]
        public string Operator2MobNo { get; set; }
        [StringLength(50)]
        public string Operator3MobNo { get; set; }
        [StringLength(50)]
        public string Operator4MobNo { get; set; }
        [StringLength(50)]
        public string Operator5MobNo { get; set; }
        [Column("TempSensorHighTH", TypeName = "decimal(18, 2)")]
        public decimal? TempSensorHighTh { get; set; }
        [Column("TempSensorLowTH", TypeName = "decimal(18, 2)")]
        public decimal? TempSensorLowTh { get; set; }
        [Column("VBATLowTHVoltage", TypeName = "decimal(18, 2)")]
        public decimal? VbatlowThvoltage { get; set; }
        public int? AlarmInterval { get; set; }
        [Column("FUAskInterval")]
        public int? FuaskInterval { get; set; }
        [Column("MAXIOCurrent", TypeName = "decimal(18, 2)")]
        public decimal? Maxiocurrent { get; set; }
        [Column("LogAutoTxENDIS")]
        public bool? LogAutoTxEndis { get; set; }
        public bool? Debug { get; set; }
        public bool? Warning { get; set; }
        public bool? Error { get; set; }
        public bool? Info { get; set; }
        public int? Direction { get; set; }
        public int? StatusStoreDuration { get; set; }
        [Column("GSMPortEN")]
        public bool? GsmportEn { get; set; }
        [Column("SCHTransferedEnableDis")]
        public bool? SchtransferedEnableDis { get; set; }
        public bool? ForceAlarmDisable { get; set; }
        [Column("COMDelay")]
        public int? Comdelay { get; set; }
        [Column("LoRaConcentratorEN")]
        public bool? LoRaConcentratorEn { get; set; }
        [Column("SettingRFPort")]
        public int? SettingRfport { get; set; }
        [Column("Card_no_1")]
        public int? CardNo1 { get; set; }
        [Column("C1_type")]
        public int? C1Type { get; set; }
        [Column("C1_debug")]
        public int? C1Debug { get; set; }
        [Column("C1_warning")]
        public int? C1Warning { get; set; }
        [Column("C1_error")]
        public int? C1Error { get; set; }
        [Column("C1_info")]
        public int? C1Info { get; set; }
        [Column("C1_sleep_en")]
        public int? C1SleepEn { get; set; }
        [Column("C1_Auo_Status_En")]
        public int? C1AuoStatusEn { get; set; }
        [Column("C1_Auto_Status_Int")]
        public int? C1AutoStatusInt { get; set; }
        [Column("C1_Log_Int_Sec")]
        public int? C1LogIntSec { get; set; }
        [Column("C1_Firmare_ver")]
        public int? C1FirmareVer { get; set; }
        [Column("C1_Saftey_Timeout_Min")]
        public int? C1SafteyTimeoutMin { get; set; }
        [Column("C1_Settings")]
        public string C1Settings { get; set; }
        [Column("Card_no_2")]
        public int? CardNo2 { get; set; }
        [Column("C2_type")]
        public int? C2Type { get; set; }
        [Column("C2_debug")]
        public int? C2Debug { get; set; }
        [Column("C2_warning")]
        public int? C2Warning { get; set; }
        [Column("C2_error")]
        public int? C2Error { get; set; }
        [Column("C2_info")]
        public int? C2Info { get; set; }
        [Column("C2_sleep_en")]
        public int? C2SleepEn { get; set; }
        [Column("C2_Auo_Status_En")]
        public int? C2AuoStatusEn { get; set; }
        [Column("C2_Auto_Status_Int")]
        public int? C2AutoStatusInt { get; set; }
        [Column("C2_Log_Int_Sec")]
        public int? C2LogIntSec { get; set; }
        [Column("C2_Firmare_ver")]
        public int? C2FirmareVer { get; set; }
        [Column("C2_Saftey_Timeout_Min")]
        public int? C2SafteyTimeoutMin { get; set; }
        [Column("C2_Settings")]
        public string C2Settings { get; set; }
        [Column("Card_no_3")]
        public int? CardNo3 { get; set; }
        [Column("C3_type")]
        public int? C3Type { get; set; }
        [Column("C3_debug")]
        public int? C3Debug { get; set; }
        [Column("C3_warning")]
        public int? C3Warning { get; set; }
        [Column("C3_error")]
        public int? C3Error { get; set; }
        [Column("C3_info")]
        public int? C3Info { get; set; }
        [Column("C3_sleep_en")]
        public int? C3SleepEn { get; set; }
        [Column("C3_Auo_Status_En")]
        public int? C3AuoStatusEn { get; set; }
        [Column("C3_Auto_Status_Int")]
        public int? C3AutoStatusInt { get; set; }
        [Column("C3_Log_Int_Sec")]
        public int? C3LogIntSec { get; set; }
        [Column("C3_Firmare_ver")]
        public int? C3FirmareVer { get; set; }
        [Column("C3_Saftey_Timeout_Min")]
        public int? C3SafteyTimeoutMin { get; set; }
        [Column("C3_Settings")]
        public string C3Settings { get; set; }
        public int? ProgramEndDayMode { get; set; }
        public int? NodeId { get; set; }
        public int? GwSrn { get; set; }
        public int? ProductId { get; set; }
        public string TagName { get; set; }
        public string NetworkTagName { get; set; }
        public string NodeTagName { get; set; }
    }
}
