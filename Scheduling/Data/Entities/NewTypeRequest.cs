using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class NewTypeRequest
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }
        public int MetadataTableId { get; set; }
        [Required]
        [StringLength(200)]
        public string NewTypeName { get; set; }
        [Required]
        [StringLength(200)]
        public string Description { get; set; }
        public bool? Acknowledge { get; set; }
        [StringLength(128)]
        public string AcknowledgeByUserId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime RequestDateTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AcknowledgeDateTime { get; set; }
        [StringLength(128)]
        public string ProjectAdminUserId { get; set; }
    }
}
