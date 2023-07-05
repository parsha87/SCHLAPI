
using Scheduling.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class SequenceWeeklyScheduleViewModel
    {
        public int SeqWeeklyId { get; set; }
        public int WeekDayId { get; set; }
        public int? PrjId { get; set; }
        public int? PrgId { get; set; }
        public int? SeqId { get; set; }
        public WeekDaysViewModel WeekDay { get; set; } = new WeekDaysViewModel();
    }
}
