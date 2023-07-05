using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class SequenceErrDetails
    {
        [Key]
        public int ErrorId { get; set; }
        public int SeqId { get; set; }
        public int PrjId { get; set; }
        public int PrgId { get; set; }
        public int NetworkId { get; set; }
        public int ZoneId { get; set; }
        public string ErrorDetail { get; set; }
        public bool? IsError { get; set; }

        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(Project.SequenceErrDetails))]
        public virtual Project Prj { get; set; }
        [ForeignKey(nameof(SeqId))]
        [InverseProperty(nameof(Sequence.SequenceErrDetails))]
        public virtual Sequence Seq { get; set; }
    }
}
