using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class SequenceWeeklySchedule
    {
        [Key]
        public int SeqWeeklyId { get; set; }
        [Key]
        public int WeekDayId { get; set; }
        public int? PrjId { get; set; }
        public int? PrgId { get; set; }
        public int? SeqId { get; set; }

        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(Project.SequenceWeeklySchedule))]
        public virtual Project Prj { get; set; }
        [ForeignKey(nameof(SeqId))]
        [InverseProperty(nameof(Sequence.SequenceWeeklySchedule))]
        public virtual Sequence Seq { get; set; }
        [ForeignKey(nameof(WeekDayId))]
        [InverseProperty(nameof(WeekDays.SequenceWeeklySchedule))]
        public virtual WeekDays WeekDay { get; set; }
    }
}
