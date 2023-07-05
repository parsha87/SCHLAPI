using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiNodeLatLong
    {
        [Key]
        public int Id { get; set; }
        public int? NetworkNo { get; set; }
        public int? NodeId { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal? Longitude { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal? ManualLatitude { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal? ManualLongitude { get; set; }
    }
}
