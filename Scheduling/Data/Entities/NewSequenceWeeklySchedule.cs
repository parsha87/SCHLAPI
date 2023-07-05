using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class NewSequenceWeeklySchedule
    {
        [Key]
        public int SeqWeeklyId { get; set; }
        [Key]
        public int WeekDayId { get; set; }
        public int? PrjId { get; set; }
        public int? PrgId { get; set; }
        public int? SeqId { get; set; }
    }
}
