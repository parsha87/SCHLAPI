using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Timezone
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        public string Identifier { get; set; }
        [StringLength(100)]
        public string StandardName { get; set; }
        [StringLength(100)]
        public string DisplayName { get; set; }
        [StringLength(100)]
        public string DaylightName { get; set; }
        public bool? SupportsDaylightSavingTime { get; set; }
        public int? BaseUtcOffsetSec { get; set; }
    }
}
