using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    [Table("BSTEventsError")]
    public partial class BsteventsError
    {
        public int? ErrorId { get; set; }
        public string Error { get; set; }
    }
}
