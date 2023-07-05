using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class FieldTechNetElement
    {
        [Key]
        [Column("FTElId")]
        public int FtelId { get; set; }
        public int? FieldTechId { get; set; }
        public int? NetworkId { get; set; }
    }
}
