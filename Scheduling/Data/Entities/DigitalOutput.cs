using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DigitalOutput
    {
        [Key]
        [Column("DOId")]
        public int Doid { get; set; }
        [Required]
        [StringLength(200)]
        public string Type { get; set; }
        public bool? Active { get; set; }
        public int? EqpTypeId { get; set; }
        [Column("GlobalLIBId")]
        public int? GlobalLibid { get; set; }
    }
}
