using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class HandHeldDevice
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public int HandHeldDevId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [StringLength(5)]
        public string StartTime { get; set; }
        public int? ActiveDuration { get; set; }
        [Column("AccessBlockorRTU")]
        public int? AccessBlockorRtu { get; set; }
        public int? Number { get; set; }
    }
}
