using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class CropTypes
    {
        [Key]
        public int CropId { get; set; }
        [Required]
        [StringLength(255)]
        public string CropName { get; set; }
    }
}
