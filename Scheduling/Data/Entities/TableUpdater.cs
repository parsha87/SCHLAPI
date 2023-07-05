using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class TableUpdater
    {
        [Key]
        public int UpdaterId { get; set; }
        public string TableName { get; set; }
        public int? UpdateVersion { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
    }
}
