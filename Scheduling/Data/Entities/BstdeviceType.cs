using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("BSTDeviceType")]
    public partial class BstdeviceType
    {
        [Key]
        public int Id { get; set; }
        [StringLength(200)]
        public string DeviceType { get; set; }
        public int? DeviceId { get; set; }
    }
}
