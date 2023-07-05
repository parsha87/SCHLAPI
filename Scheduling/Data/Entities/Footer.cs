using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Footer
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string FooterText { get; set; }
    }
}
