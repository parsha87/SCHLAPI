using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class WeekDays
    {
        public WeekDays()
        {
            DummySeqWeeklySch = new HashSet<DummySeqWeeklySch>();
            SequenceWeeklySchedule = new HashSet<SequenceWeeklySchedule>();
        }

        [Key]
        public int WeekDayId { get; set; }
        [StringLength(10)]
        public string Name { get; set; }

        [InverseProperty("WeekDay")]
        public virtual ICollection<DummySeqWeeklySch> DummySeqWeeklySch { get; set; }
        [InverseProperty("WeekDay")]
        public virtual ICollection<SequenceWeeklySchedule> SequenceWeeklySchedule { get; set; }
    }
}
