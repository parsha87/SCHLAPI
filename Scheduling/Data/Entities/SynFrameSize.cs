using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class SynFrameSize
    {
        [StringLength(50)]
        public string FrameName { get; set; }
        public int? FixedHeader { get; set; }
        public int? VariableHeader { get; set; }
    }
}
