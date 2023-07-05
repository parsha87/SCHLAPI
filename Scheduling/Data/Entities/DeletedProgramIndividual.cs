using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scheduling.Data.Entities
{
    public partial class DeletedProgramIndividual
    {
        [Key]
        public int Id { get; set; }
        public int ProgId { get; set; }
        public int PrjId { get; set; }
    }
}
