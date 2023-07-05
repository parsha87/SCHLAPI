using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("dummySeqWeeklySch")]
    public partial class DummySeqWeeklySch
    {
        [Key]
        public int SeqWeeklyId { get; set; }
        [Key]
        public int WeekDayId { get; set; }
        public int? SeqId { get; set; }

        [ForeignKey(nameof(SeqId))]
        [InverseProperty(nameof(DummySequence.DummySeqWeeklySch))]
        public virtual DummySequence Seq { get; set; }
        [ForeignKey(nameof(WeekDayId))]
        [InverseProperty(nameof(WeekDays.DummySeqWeeklySch))]
        public virtual WeekDays WeekDay { get; set; }
    }
}
