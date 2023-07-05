using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class UnitType
    {
        [Key]
        public int UnitTypeId { get; set; }
        public int? EqpTypeId { get; set; }
        [StringLength(200)]
        public string UnitTypeName { get; set; }
        [Column("XMLFileName")]
        [StringLength(200)]
        public string XmlfileName { get; set; }
        public bool? Active { get; set; }
        [Column("GlobalLIBId")]
        public int? GlobalLibid { get; set; }
    }
}
