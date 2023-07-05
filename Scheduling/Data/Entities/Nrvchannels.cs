using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("NRVChannels")]
    public partial class Nrvchannels
    {
        [Key]
        public int Id { get; set; }
        public int? NetworkId { get; set; }
        [Column("RTUNo")]
        public int? Rtuno { get; set; }
        public int? RtuId { get; set; }
        public int? ValveNo { get; set; }
        [StringLength(300)]
        public string ChannelName { get; set; }
        public int? UpIds { get; set; }
    }
}
