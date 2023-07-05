using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ProjectElements
    {
        [Key]
        public int PrjId { get; set; }
        [Key]
        public int TypeId { get; set; }
        [Key]
        [StringLength(2)]
        public string TypeChar { get; set; }

        [ForeignKey(nameof(PrjId))]
        [InverseProperty(nameof(Project.ProjectElements))]
        public virtual Project Prj { get; set; }
    }
}
