using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class Root
    {
        public List<dynamic> HEAD { get; set; }
        public EventBody BODY { get; set; }
    }
    public class EventBody
    {
        public List<dynamic> DATA { get; set; }
    }




}
