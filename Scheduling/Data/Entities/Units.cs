using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Units
    {
        public Units()
        {
            Project = new HashSet<Project>();
        }

        [Key]
        public int UnitsId { get; set; }
        [StringLength(100)]
        public string Name { get; set; }

        [InverseProperty("Units")]
        public virtual ICollection<Project> Project { get; set; }
    }
}
