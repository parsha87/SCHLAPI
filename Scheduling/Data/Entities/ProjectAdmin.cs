using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ProjectAdmin
    {
        [Key]
        public int PrjId { get; set; }
        [Key]
        [StringLength(128)]
        public string UserId { get; set; }

        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(Project.ProjectAdmin))]
        public virtual Project Prj { get; set; }
    }
}
