using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class NotiAlerts
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        public int? NetworkId { get; set; }
        public int? RequestType { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ReceivedDateTime { get; set; }
        public string FrameResponse { get; set; }
        [Column("IsSMSSend")]
        public int? IsSmssend { get; set; }
    }
}
