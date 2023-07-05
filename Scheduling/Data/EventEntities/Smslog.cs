using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    [Table("SMSLog")]
    public partial class Smslog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(128)]
        public string LoginUserId { get; set; }
        [Required]
        [StringLength(128)]
        public string SmsToSendUserId { get; set; }
        [Required]
        public string SmsText { get; set; }
        [Column("SMSSendDateTime", TypeName = "datetime")]
        public DateTime? SmssendDateTime { get; set; }
        public string ResponsefromServer { get; set; }
    }
}
