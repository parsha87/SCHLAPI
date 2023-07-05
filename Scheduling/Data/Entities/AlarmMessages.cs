using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class AlarmMessages
    {
        [Key]
        [Column("AlarmMSGId")]
        public int AlarmMsgid { get; set; }
        public int ChannelId { get; set; }
        public int AlarmLevel { get; set; }
        [StringLength(200)]
        public string Message { get; set; }
    }
}
