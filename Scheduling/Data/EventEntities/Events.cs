using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class Events
    {
        public int? PrjId { get; set; }
        public int? NetworkId { get; set; }
        [Key]
        public int EventId { get; set; }
        public int? ObjTypeId { get; set; }
        [StringLength(1)]
        public string ActionExecuted { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AddedDateTime { get; set; }
        public int? Priority { get; set; }
        [Column("IsSentToBST")]
        public bool? IsSentToBst { get; set; }
        [Column("BSTSentDateTime", TypeName = "datetime")]
        public DateTime? BstsentDateTime { get; set; }
        [Column("ObjectIdInDB")]
        public int? ObjectIdInDb { get; set; }
        [Column("ValveScheduleInDB")]
        [StringLength(100)]
        public string ValveScheduleInDb { get; set; }
        [StringLength(15)]
        public string Status { get; set; }
        public int? StatusFrameId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StatusTimeStamp { get; set; }
        [Column("statusrtuidinnw")]
        public int? Statusrtuidinnw { get; set; }
        [Column("IsSMSSend")]
        public bool? IsSmssend { get; set; }

        [ForeignKey(nameof(ObjTypeId))]
        [InverseProperty(nameof(EventObjectTypes.Events))]
        public virtual EventObjectTypes ObjType { get; set; }
    }
}
