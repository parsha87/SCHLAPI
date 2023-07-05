using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class SchemaChangesLog
    {
        [Key]
        [Column("SchemaChangeLogID")]
        public int SchemaChangeLogId { get; set; }
        [Required]
        [StringLength(2)]
        public string MajorReleaseNumber { get; set; }
        [Required]
        [StringLength(2)]
        public string MinorReleaseNumber { get; set; }
        [Required]
        [StringLength(4)]
        public string PointReleaseNumber { get; set; }
        [Required]
        [StringLength(50)]
        public string ScriptName { get; set; }
        [Required]
        public string ScriptDescription { get; set; }
        [Required]
        public string Script { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateApplied { get; set; }
    }
}
