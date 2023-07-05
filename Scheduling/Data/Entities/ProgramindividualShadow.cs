using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    [Table("PROGRAMINDIVIDUAL_SHADOW")]
    public partial class ProgramindividualShadow
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("PROGINDID")]
        public int Progindid { get; set; }
        [Column("PRJID")]
        public int? Prjid { get; set; }
        [Column("PROGID")]
        public int? Progid { get; set; }
        [Column("STARTDATE", TypeName = "datetime")]
        public DateTime? Startdate { get; set; }
        [Column("STARTTIME")]
        [StringLength(10)]
        public string Starttime { get; set; }
        [Column("ENDDATE", TypeName = "datetime")]
        public DateTime? Enddate { get; set; }
        [Column("ENDTIME")]
        [StringLength(10)]
        public string Endtime { get; set; }
        [Column("ISLOOPING")]
        public bool? Islooping { get; set; }
        [Column("ISLOCKED")]
        public bool? Islocked { get; set; }
        [Column("Shadow_SDT", TypeName = "datetime")]
        public DateTime ShadowSdt { get; set; }
        [Column("Shadow_EDT", TypeName = "datetime")]
        public DateTime ShadowEdt { get; set; }
        [Required]
        [Column("ACTIONTYPE")]
        [StringLength(1)]
        public string Actiontype { get; set; }
        [Required]
        [Column("USERID")]
        [StringLength(128)]
        public string Userid { get; set; }
    }
}
