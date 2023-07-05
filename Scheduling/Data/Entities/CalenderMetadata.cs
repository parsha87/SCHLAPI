using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class CalenderMetadata
    {
        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }
        [Column("Day of month")]
        public double? DayOfMonth { get; set; }
        public double? DayYear { get; set; }
        public double? DayofWeek { get; set; }
        [Column("isMonday")]
        [StringLength(255)]
        public string IsMonday { get; set; }
        [Column("isTuesday")]
        [StringLength(255)]
        public string IsTuesday { get; set; }
        [Column("isWednesday")]
        [StringLength(255)]
        public string IsWednesday { get; set; }
        [Column("isThursday")]
        [StringLength(255)]
        public string IsThursday { get; set; }
        [Column("isFriay")]
        [StringLength(255)]
        public string IsFriay { get; set; }
        [Column("isSaturday")]
        [StringLength(255)]
        public string IsSaturday { get; set; }
        [Column("isSunday")]
        [StringLength(255)]
        public string IsSunday { get; set; }
        [Column("is Holiday")]
        [StringLength(255)]
        public string IsHoliday { get; set; }
        [Column("isLeapYear")]
        [StringLength(255)]
        public string IsLeapYear { get; set; }
        [Column("isWorkingDay5DaysWeek")]
        [StringLength(255)]
        public string IsWorkingDay5DaysWeek { get; set; }
        [Column("isWorkingDay6DaysWeek")]
        [StringLength(255)]
        public string IsWorkingDay6DaysWeek { get; set; }
    }
}
