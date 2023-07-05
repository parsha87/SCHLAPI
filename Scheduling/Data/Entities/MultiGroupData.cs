using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiGroupData
    {
        [Key]
        public int Id { get; set; }
        public int? GroupId { get; set; }
        public string SensorName { get; set; }
        public string SensorTagName { get; set; }
        public int? ProductType { get; set; }
        public int? Gwsrn { get; set; }
        public int? NodeProductId { get; set; }
        public int? NodeId { get; set; }
        [Column("SSNo")]
        [StringLength(10)]
        public int? Ssno { get; set; }
        public int? ConfigurationId { get; set; }
        public int? Priority { get; set; }
    }
}
