using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class FilterViewModel
    {
       
    }


    public class FilterZone
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public List<KeyValueViewModel> Groups { get; set; } = new List<KeyValueViewModel>();
        public List<KeyValueViewModel> Rtus { get; set; } = new List<KeyValueViewModel>();

    }

}
