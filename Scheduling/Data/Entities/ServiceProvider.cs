using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ServiceProvider
    {
        [Key]
        public int ServiceProviderId { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
    }
}
