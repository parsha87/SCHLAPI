using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class MultiSequenceUploading
    {
        [Key]
        public int Id { get; set; }
        public bool? SeqUploadingFlag { get; set; }
    }
}
