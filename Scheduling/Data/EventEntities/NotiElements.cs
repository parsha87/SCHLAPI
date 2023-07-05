using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class NotiElements
    {
        public int Id { get; set; }
        [StringLength(15)]
        public string Name { get; set; }
        [StringLength(1)]
        public string Action { get; set; }
        [Key]
        public int ActionListId { get; set; }
    }
}
