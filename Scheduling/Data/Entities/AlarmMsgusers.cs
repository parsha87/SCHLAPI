using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("AlarmMSGUsers")]
    public partial class AlarmMsgusers
    {
        [Key]
        [Column("AlarmMSGUsersId")]
        public int AlarmMsgusersId { get; set; }
        [Column("AlarmMSGId")]
        public int AlarmMsgid { get; set; }
        [Required]
        [StringLength(128)]
        public string AspNetUsersId { get; set; }
    }
}
