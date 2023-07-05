using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class ImportUserLog
    {
        [Key]
        public int FileNo { get; set; }
        public int? PrjId { get; set; }
        [StringLength(200)]
        public string FileName { get; set; }
        [StringLength(200)]
        public string UploadedFolderPath { get; set; }
        [StringLength(200)]
        public string UploadedFileName { get; set; }
        [StringLength(130)]
        public string UserName { get; set; }
        [StringLength(1)]
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateUploaded { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateProcessed { get; set; }
        public string FailuareReason { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateProcessStart { get; set; }
        public int? NoOfUserToImport { get; set; }
    }
}
