using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class AlarmLevelAction
    {
        public int? AlarmLevel { get; set; }
        public int? ActionId { get; set; }
    }
}
