using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class SequenceMasterConfigViewModel
    {
        public int MstseqId { get; set; }
        public int? SeqId { get; set; }
        public int? ProjectId { get; set; }
        public int? PrgId { get; set; }
        public int? NetworkId { get; set; }
        public int? ZoneId { get; set; }
        public int? StartId { get; set; }
        [MaxLength(10)]
        public string StartTime { get; set; }
    }
}
