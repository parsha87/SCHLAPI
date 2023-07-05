using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class ManualOverrideMasterHistory
    {
        [Key]
        public int HistoryMoId { get; set; }
        public int? MoId { get; set; }
        [Column("MOTypeid")]
        public int? Motypeid { get; set; }
        [StringLength(128)]
        public string UserName { get; set; }
        [Column("MOCreatedDate", TypeName = "datetime")]
        public DateTime? MocreatedDate { get; set; }
        public int? OverrideForId { get; set; }
        public int? ActionTypeId { get; set; }
        [Column("alarmLevel")]
        public int? AlarmLevel { get; set; }
        [StringLength(1)]
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExecutionDatetime { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        public int? BlockId { get; set; }
        [Column("MOStartDateTime", TypeName = "datetime")]
        public DateTime? MostartDateTime { get; set; }
        [Column("MOEndDateTime", TypeName = "datetime")]
        public DateTime? MoendDateTime { get; set; }
        [Column("MOName")]
        [StringLength(50)]
        public string Moname { get; set; }
        public bool? IsDeleted { get; set; }
        [StringLength(25)]
        public string TagName { get; set; }
        public bool? MasterValveOpEnabled { get; set; }
    }
}
