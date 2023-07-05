using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiValveEventRawProcessing
    {
        [Key]
        public int Id { get; set; }
        public string Frame { get; set; }
        public bool? IsProcessed { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AddedDateTime { get; set; }
    }
}
