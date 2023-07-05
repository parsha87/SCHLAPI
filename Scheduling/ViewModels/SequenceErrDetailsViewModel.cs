using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class SequenceErrDetailsViewModel
    {
        public int ErrorId { get; set; }
        public int SeqId { get; set; }
        public int PrjId { get; set; }
        public int PrgId { get; set; }
        public int NetworkId { get; set; }
        public int ZoneId { get; set; }
        public string ErrorDetail { get; set; }
        public bool? IsError { get; set; }
    }
}
