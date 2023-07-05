using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("StatusBSTLog")]
    public partial class StatusBstlog
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LogDateTime { get; set; }
        public int? BstId { get; set; }
        public int? NetworkId { get; set; }
        public int? DeviceTypeId { get; set; }
        public string MessageBody { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ProcessedDateTime { get; set; }
    }
}
