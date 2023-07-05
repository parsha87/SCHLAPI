using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Scheduling.ViewModels
{
    public class ProgramIndividualViewModel
    {
        [Required]
        public int ProgIndId { get; set; }
        [Required]
        public int? PrjId { get; set; }
        [Required]
        public int? ProgId { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        [StringLength(10)]
        [Required]
        public string StartTime { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }
        [Required]
        [StringLength(10)]
        public string EndTime { get; set; }
        public bool? IsLooping { get; set; }
        public bool? IsActualLoop { get; set; }
        public bool? IsLocked { get; set; }

        public string Name { get; set; }
    }
}
