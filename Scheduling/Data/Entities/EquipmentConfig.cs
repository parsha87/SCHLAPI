using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class EquipmentConfig
    {
        [Key]
        public int EquipmentConfigId { get; set; }
        public int? ConfigForEqpId { get; set; }
        [StringLength(10)]
        public string EqpTypeId { get; set; }
        [StringLength(1000)]
        public string FieldName { get; set; }
        [StringLength(100)]
        public string FieldType { get; set; }
        public int? TypeId { get; set; }
        public int? SubTypeId { get; set; }
        public int? DisplayOrderNo { get; set; }
        public bool? IsConsideredForRule { get; set; }
        public bool? IsMandatory { get; set; }
    }
}
