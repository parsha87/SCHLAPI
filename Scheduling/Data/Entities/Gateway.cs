using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Gateway
    {
        [Key]
        public int Id { get; set; }
        public int? GatewayNo { get; set; }
        public int? SerialNo { get; set; }
        [Column(TypeName = "decimal(18, 6)")]
        public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(18, 6)")]
        public decimal? Longitude { get; set; }
        public bool? IsActive { get; set; }
        public string TagName { get; set; }
    }
}
