using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class UpdateIdsProject
    {
        [Key]
        public int Id { get; set; }
        [Column("GWId")]
        public int? Gwid { get; set; }
        public int? ProjectUpId { get; set; }
    }
}
