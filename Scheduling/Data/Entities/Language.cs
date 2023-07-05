using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class Language
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string LanguageText { get; set; }
        [Required]
        [StringLength(255)]
        public string LanguageValue { get; set; }
    }
}
