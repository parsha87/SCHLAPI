using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class StatusFrameValveData
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public int NetworkId { get; set; }
        [Key]
        [Column("RTUIdInNW")]
        public int RtuidInNw { get; set; }
        [Key]
        public int TypeId { get; set; }
        [Key]
        public int EqpTypeId { get; set; }
        [Key]
        public int EqpId { get; set; }
        [Key]
        [Column("RTUTimeStamp", TypeName = "datetime")]
        public DateTime RtutimeStamp { get; set; }
        [Key]
        public int SlotId { get; set; }
        [StringLength(2)]
        public string ValveStatus { get; set; }
        public int? PrjId { get; set; }
    }
}
