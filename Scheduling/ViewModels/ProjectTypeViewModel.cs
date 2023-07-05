using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class ProjectTypeViewModel
    {
        public int PrjTypeId { get; set; }
        [MaxLength(200)]
        public string Type { get; set; }
        public bool? Active { get; set; }
    }
}
