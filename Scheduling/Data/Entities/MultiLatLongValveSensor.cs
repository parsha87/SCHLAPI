using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiLatLongValveSensor
    {
        [Key]
        public int Id { get; set; }
        public int? NodeId { get; set; }
        public int? NodeNo { get; set; }
        public int? NetworkNo { get; set; }
        public int? GatewayNo { get; set; }
        public int? ElementNo { get; set; }
        [Column("VSType")]
        public int? Vstype { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal? Longitude { get; set; }
    }
}
