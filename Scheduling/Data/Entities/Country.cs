using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Country
    {
        [Key]
        public int CntryId { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
    }
}
