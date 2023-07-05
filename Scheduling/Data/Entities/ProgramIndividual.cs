using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ProgramIndividual
    {
        [Key]
        public int ProgIndId { get; set; }
        public int? PrjId { get; set; }
        public int? NetworkId { get; set; }
        public int? ProgId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [StringLength(10)]
        public string StartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        [StringLength(10)]
        public string EndTime { get; set; }
        public bool? IsLooping { get; set; }
        public bool? IsActualLoop { get; set; }
        public bool? IsLocked { get; set; }
    }
}
