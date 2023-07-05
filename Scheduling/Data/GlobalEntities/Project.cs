using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.GlobalEntities
{
    public partial class Project
    {
        [Key]
        public int Id { get; set; }
        public int? ProjectNo { get; set; }
        [StringLength(500)]
        public string ProjectName { get; set; }
        public string ProjectUrl { get; set; }
    }
}
