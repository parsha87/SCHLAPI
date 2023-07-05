using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class CardType
    {
        [Key]
        public int Id { get; set; }
        [Column("CardType")]
        [StringLength(500)]
        public string CardType1 { get; set; }
        public int? CardNo { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
    }
}
