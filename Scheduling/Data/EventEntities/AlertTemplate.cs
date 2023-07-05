using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class AlertTemplate
    {
        [Key]
        public int AlertTextId { get; set; }
        public string AlertText { get; set; }
        public int? VariableCount { get; set; }
    }
}
