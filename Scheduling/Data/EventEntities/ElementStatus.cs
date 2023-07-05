using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class ElementStatus
    {
        [Key]
        public int EleStatusId { get; set; }
        public string Status { get; set; }
    }
}
