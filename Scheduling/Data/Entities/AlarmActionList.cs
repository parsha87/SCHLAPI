using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class AlarmActionList
    {
        [Key]
        public int ActionId { get; set; }
        [StringLength(100)]
        public string Action { get; set; }
        [StringLength(50)]
        public string Param { get; set; }
        [StringLength(100)]
        public string MoreInfo { get; set; }
    }
}
