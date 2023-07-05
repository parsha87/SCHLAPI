using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    [Table("BSTEventsConfig")]
    public partial class BsteventsConfig
    {
        [Key]
        [Column("BSTEventsEleId")]
        public int BsteventsEleId { get; set; }
        [Column("BSTEventId")]
        public int? BsteventId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EventDateTime { get; set; }
        [StringLength(10)]
        public string EleNumberandType { get; set; }
        public int? Number { get; set; }
        public int? EleStatus { get; set; }
        public int? Reason { get; set; }
        public int? ErrorCode { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ReceivedDateTime { get; set; }
        public int? EventObjectTypeId { get; set; }
        [Column("IsSMSSend")]
        public bool? IsSmssend { get; set; }
        [StringLength(100)]
        public string EventType { get; set; }

        [ForeignKey(nameof(BsteventId))]
        [InverseProperty(nameof(Bstevents.BsteventsConfig))]
        public virtual Bstevents Bstevent { get; set; }
        [ForeignKey(nameof(EventObjectTypeId))]
        [InverseProperty(nameof(EventObjectTypes.BsteventsConfig))]
        public virtual EventObjectTypes EventObjectType { get; set; }
    }
}
