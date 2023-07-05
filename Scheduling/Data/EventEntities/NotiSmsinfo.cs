using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    [Table("NotiSMSInfo")]
    public partial class NotiSmsinfo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        public int? Networkid { get; set; }
        public string NotiSetting { get; set; }
        [Column("LockedDateTIme", TypeName = "datetime")]
        public DateTime? LockedDateTime { get; set; }
        public int? ProjectId { get; set; }
        [Column("IsSMSSend")]
        public int? IsSmssend { get; set; }
    }
}
