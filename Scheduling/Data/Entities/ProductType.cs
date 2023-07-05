using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ProductType
    {
        [Key]
        public int Id { get; set; }
        [StringLength(200)]
        public string Type { get; set; }
        public int? ProductNo { get; set; }
        public int? NoOfValves { get; set; }
        [Column("NoOfSS")]
        public int? NoOfSs { get; set; }
    }
}
