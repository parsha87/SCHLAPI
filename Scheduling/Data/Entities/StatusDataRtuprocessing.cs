using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("StatusDataRTUProcessing")]
    public partial class StatusDataRtuprocessing
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ReceivedDateTime { get; set; }
        public string QueueName { get; set; }
        [Column("MessageID")]
        public string MessageId { get; set; }
        public string MessageBody { get; set; }
        public string ReservationId { get; set; }
        [StringLength(200)]
        public string Status { get; set; }
        public int? NetworkId { get; set; }
    }
}
