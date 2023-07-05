using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("MultiUIVersion")]
    public partial class MultiUiversion
    {
        public int? Id { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Version { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ChangedDatetime { get; set; }
    }
}
