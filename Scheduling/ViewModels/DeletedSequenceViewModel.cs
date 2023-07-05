using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class DeletedSequenceViewModel
    {
        public int DsqId { get; set; }
        public int? SeqNo { get; set; }
        public int? ProgramNo { get; set; }
        public bool? Reused { get; set; }
    }
}
