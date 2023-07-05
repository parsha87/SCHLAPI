using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class WeekDaysViewModel
    {
        public int WeekDayId { get; set; }
        [MaxLength(10)]
        public string Name { get; set; }
    }
}
