using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class LoopDates
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LoopStartDate { get; set; }
        [StringLength(10)]
        public string LoopStartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LoopEndDate { get; set; }
        [StringLength(10)]
        public string LoopEndTime { get; set; }
    }
}
