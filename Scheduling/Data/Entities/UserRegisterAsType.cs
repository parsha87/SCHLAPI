using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class UserRegisterAsType
    {
        [Key]
        public int RegisterAsId { get; set; }
        [Required]
        [StringLength(50)]
        public string RegisterType { get; set; }
    }
}
