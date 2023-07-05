using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class AddressDetails
    {
        public int AddrDtlId { get; set; }
        public int ExpansionCardId { get; set; }
        public bool IsExpansionCard { get; set; }
        public int SlotId { get; set; }
        public int DigitalOp { get; set; }
        public int AnalogIp { get; set; }
        public int DigitalIp { get; set; }
        public int AnalogOp { get; set; }
    }
}
