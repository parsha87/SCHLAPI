using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class BaseStation
    {
        [Key]
        [Column("BSTId")]
        public int Bstid { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public int? NetworkId { get; set; }
        public bool? PorS { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public int? MinAttendance { get; set; }
        public int? SecAttendance { get; set; }
        public int? Timeout { get; set; }
        public int? ServiceProviderId { get; set; }
        [StringLength(50)]
        public string SimNo { get; set; }
        [Column("FieldTechniciansPhNO")]
        [StringLength(20)]
        public string FieldTechniciansPhNo { get; set; }
        public int? HandHeldDeviceId { get; set; }
        public bool? Active { get; set; }
        [Column("UsedInNW")]
        public bool? UsedInNw { get; set; }
        public int? PrjId { get; set; }
        [StringLength(20)]
        public string PhoneNo { get; set; }
        [StringLength(25)]
        public string TagName { get; set; }
    }
}
