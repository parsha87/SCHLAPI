using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class EquipmentConfigValuesTemplate
    {
        [Key]
        public int Id { get; set; }
        public int ConfigforEqpId { get; set; }
        [Required]
        [StringLength(100)]
        public string FieldName { get; set; }
        [Required]
        [StringLength(50)]
        public string FieldValue { get; set; }
        public int PrjId { get; set; }
        [Required]
        [StringLength(2)]
        public string SensorType { get; set; }
        public int SensorTypeId { get; set; }
    }
}
