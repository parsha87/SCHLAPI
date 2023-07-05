using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.EventEntities
{
    public partial class RuleOffsetMaxCount
    {
        public int Id { get; set; }
        public int MaxCount { get; set; }
    }
}
