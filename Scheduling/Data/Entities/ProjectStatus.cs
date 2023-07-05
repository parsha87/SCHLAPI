using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ProjectStatus
    {
        [Key]
        public int StatusId { get; set; }
        public int PrjId { get; set; }
        public bool Status { get; set; }
        [StringLength(50)]
        public string User { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Timestamp { get; set; }
    }
}
