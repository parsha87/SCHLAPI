using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DashboardMatrixSensors
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [StringLength(200)]
        public string Element { get; set; }
        public int? ElementId { get; set; }
        public int? Sensor1 { get; set; }
        public int? Sensor2 { get; set; }
        public int? Sensor3 { get; set; }
    }
}
