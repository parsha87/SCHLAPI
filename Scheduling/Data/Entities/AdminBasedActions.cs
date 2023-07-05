using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class AdminBasedActions
    {
        [Key]
        public int Id { get; set; }
        [StringLength(128)]
        public string UserId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }
        [StringLength(128)]
        public string ActionOnUserId { get; set; }
        public string Action { get; set; }
    }
}
