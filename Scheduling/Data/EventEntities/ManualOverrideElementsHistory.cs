using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class ManualOverrideElementsHistory
    {
        [Key]
        [Column("HistoryMOElementsId")]
        public int HistoryMoelementsId { get; set; }
        [Column("MOElementsId")]
        public int MoelementsId { get; set; }
        [Column("MOId")]
        public int? Moid { get; set; }
        public int? ElementTypeId { get; set; }
        public int? ObjectId { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
