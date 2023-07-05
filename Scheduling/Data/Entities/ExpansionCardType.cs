using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ExpansionCardType
    {
        public ExpansionCardType()
        {
            Slot = new HashSet<Slot>();
        }

        [Key]
        public int ExpCardTypeId { get; set; }
        [StringLength(20)]
        public string Type { get; set; }
        public bool? Active { get; set; }
        public int? NoOfAnalogIp { get; set; }
        public int? NoOfDigitalIp { get; set; }
        public int? NoOfDigitalOp { get; set; }

        [InverseProperty("ExpCardType")]
        public virtual ICollection<Slot> Slot { get; set; }
    }
}
