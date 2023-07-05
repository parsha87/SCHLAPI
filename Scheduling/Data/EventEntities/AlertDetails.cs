using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class AlertDetails
    {
        [Key]
        [Column("AlertDId")]
        public int AlertDid { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string MobileNo { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateOfStart { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string ValveName { get; set; }
        public string TagName { get; set; }
        public string FertGrName { get; set; }
        public string FertGrTag { get; set; }
        public string FertTime { get; set; }
        public int? AlertConditionId { get; set; }
        public int? IsCall { get; set; }
        public int? ValveTimespanId { get; set; }
        public int? SeqId { get; set; }
        public string OwnerOrAccessor { get; set; }
    }
}
