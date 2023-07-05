using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ProjectDefaults
    {
        [Key]
        public int Id { get; set; }
        public int? PrjId { get; set; }
        [StringLength(200)]
        public string ElementKey { get; set; }
        [StringLength(50)]
        public string Value { get; set; }
    }
}
