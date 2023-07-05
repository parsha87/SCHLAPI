using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class NoWaterWindow
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [StringLength(10)]
        public string StartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        [StringLength(10)]
        public string EndTime { get; set; }
    }
}
