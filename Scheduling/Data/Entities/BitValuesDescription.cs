using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class BitValuesDescription
    {
        public int? Id { get; set; }
        [StringLength(20)]
        public string ChargingStatus { get; set; }
        [StringLength(20)]
        public string AuxSupplyStatus { get; set; }
        [Column("DIStatus")]
        [StringLength(20)]
        public string Distatus { get; set; }
    }
}
