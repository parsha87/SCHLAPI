using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class RecordEditProjectClick
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string MobileNo { get; set; }
        public string DateTimeOfClick { get; set; }
        public int? FlagCall { get; set; }
    }
}
