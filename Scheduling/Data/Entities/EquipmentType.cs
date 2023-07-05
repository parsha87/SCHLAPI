using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class EquipmentType
    {
        public EquipmentType()
        {
            AnalogSensorType = new HashSet<AnalogSensorType>();
            Channel = new HashSet<Channel>();
            DigitalCounter = new HashSet<DigitalCounter>();
            DigitalNonc = new HashSet<DigitalNonc>();
            Equipment = new HashSet<Equipment>();
        }

        [Key]
        public int EqpTypeId { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [StringLength(200)]
        public string Type { get; set; }
        public bool? Active { get; set; }
        [StringLength(10)]
        public string Comment { get; set; }

        [InverseProperty("EqpType")]
        public virtual ICollection<AnalogSensorType> AnalogSensorType { get; set; }
        [InverseProperty("EqpType")]
        public virtual ICollection<Channel> Channel { get; set; }
        [InverseProperty("EqpType")]
        public virtual ICollection<DigitalCounter> DigitalCounter { get; set; }
        [InverseProperty("EqpType")]
        public virtual ICollection<DigitalNonc> DigitalNonc { get; set; }
        [InverseProperty("EqpType")]
        public virtual ICollection<Equipment> Equipment { get; set; }
    }
}
