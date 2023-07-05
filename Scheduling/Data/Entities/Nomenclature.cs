using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Nomenclature
    {
        [Key]
        public int Id { get; set; }
        [Column("Nomenclature")]
        [StringLength(50)]
        public string Nomenclature1 { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
    }
}
