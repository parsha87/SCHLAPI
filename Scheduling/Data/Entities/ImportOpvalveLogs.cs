using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("ImportOPValveLogs")]
    public partial class ImportOpvalveLogs
    {
        [Key]
        public int Id { get; set; }
        [StringLength(500)]
        public string FileName { get; set; }
        [StringLength(500)]
        public string UploadedFolderPath { get; set; }
        [StringLength(500)]
        public string UploadedFileName { get; set; }
        [StringLength(10)]
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateUploaded { get; set; }
        public int? PrjId { get; set; }
        public string UserName { get; set; }
    }
}
