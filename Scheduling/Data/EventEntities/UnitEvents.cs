using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    [Table("Unit_Events")]
    public partial class UnitEvents
    {
        public int? PrjId { get; set; }
        [Key]
        public int EventId { get; set; }
        public int? ObjTypeId { get; set; }
        [StringLength(1)]
        public string ActionExecuted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AddedDateTime { get; set; }
        public int? ObjectIdInDb { get; set; }
    }
}
