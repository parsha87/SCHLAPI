using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ProjectType
    {
        public ProjectType()
        {
            DummySequence = new HashSet<DummySequence>();
            Sequence = new HashSet<Sequence>();
        }

        [Key]
        public int PrjTypeId { get; set; }
        [StringLength(200)]
        public string Type { get; set; }
        public bool? Active { get; set; }

        [InverseProperty("PrjType")]
        public virtual ICollection<DummySequence> DummySequence { get; set; }
        [InverseProperty("PrjType")]
        public virtual ICollection<Sequence> Sequence { get; set; }
    }
}
