using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduling.ViewModels
{
    public class KeyValuePairViewModel
    {
        public int Value { get; set; }
        public string Text { get; set; }
        public string TagName { get; set; }
        public bool? IsFlushRelated { get; set; }
        public bool? IsFertilizerRelated { get; set; }
    }

    public class KeyValuePairGroupViewModel
    {
        public int Value { get; set; }
        public string Text { get; set; }
        public string TagName { get; set; }
        public List<KeyValuePairViewModel> Channels { get; set; } = new List<KeyValuePairViewModel>();
    }

    public class KeyValueViewModel
    {
        public int Value { get; set; }
        public string Text { get; set; }
        public string TagName { get; set; }
    }


    public class KeyValueBlockSensorViewModel
    {
        public int Value { get; set; }
        public string Text { get; set; }
        public string TagName { get; set; }
        public List<dynamic> MatrixAssignedSensor { get; set; } = new List<dynamic>();

    }

    public class KeyValueZB
    {
        public int ZoneId { get; set; }
        public string ZoneName { get; set; }
        public List<KeyValueBS> Blocks { get; set; } = new List<KeyValueBS>();

    }

    public class KeyValueBS
    {
        public int Value { get; set; }
        public string Text { get; set; }
        public List<KeyValueViewModel> SubBlocks { get; set; } = new List<KeyValueViewModel>();

    }




    public class ZBSList
    {
        public List<KeyValueZB> ZoneBlockSubblock { get; set; } = new List<KeyValueZB>();
    }

}
