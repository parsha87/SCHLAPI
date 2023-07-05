using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class NewSequence
    {
        [Key]
        public int SeqId { get; set; }
        public int PrjId { get; set; }
        public int PrgId { get; set; }
        public int NetworkId { get; set; }
        public int ZoneId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SeqStartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SeqEndDate { get; set; }
        public bool? IsProgrammable { get; set; }
        [Column("BasisOfOP")]
        [StringLength(10)]
        public string BasisOfOp { get; set; }
        public int? IntervalDays { get; set; }
        public int? PrjTypeId { get; set; }
        public int? OperationTypeId { get; set; }
        [Column("MannualorAutoET")]
        [StringLength(1)]
        public string MannualorAutoEt { get; set; }
        [Column("IsSmartET")]
        public bool? IsSmartEt { get; set; }
        [StringLength(50)]
        public string SeqName { get; set; }
        public bool? ValidationState { get; set; }
        public bool? IsValid { get; set; }
        public int? SeqNo { get; set; }
        [StringLength(200)]
        public string SeqDesc { get; set; }
        [StringLength(25)]
        public string SeqTagName { get; set; }
        [StringLength(3)]
        public string SeqType { get; set; }
        public int? IsSent { get; set; }
        public bool? IsDeleting { get; set; }
        [Column("StartTIme")]
        [StringLength(25)]
        public string StartTime { get; set; }
        public int? Upid { get; set; }
    }
}
